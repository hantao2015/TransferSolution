using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperSocket.Common
{
    /// <summary>
    /// TLV实体
    /// </summary>
    public class TLVEntity
    {
        /// <summary>
        /// 标记
        /// </summary>
        public byte[] Tag { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public byte[] Length { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// 标记占用字节数
        /// </summary>
        public int TagSize { get; set; }

        /// <summary>
        /// 数据长度占用字节数
        /// </summary>
        public int LengthSize { get; set; }

        /// <summary>
        /// 子嵌套TLV实体
        /// </summary>
        public TLVEntity Sub_TLVEntity { get; set; }
    }

    /// <summary>
    /// TLV打包类
    /// </summary>
    public class TLVPackage
    {
        /// <summary>
        /// 构造TLV
        /// </summary>
        /// <param name="buffer"></param>
        public static List<TLVEntity> Construct(byte[] buffer)
        {
            List<TLVEntity> list = new List<TLVEntity>();
            int currentIndex = 0;
            int currentStatus = 'T'; //状态字符
            int valueSize = 0;

            TLVEntity tlvEntity = null;

            while (currentIndex < buffer.Length)
            {
                switch (currentStatus)
                {
                    case 'T':
                        tlvEntity = new TLVEntity();
                        valueSize = 0;
                        //判断是否单一结构
                        if ((buffer[currentIndex] & 0x20) != 0x20)
                        {
                            tlvEntity.Sub_TLVEntity = null; //单一结构时将子Tag置空【】
                            //判断是否多字节Tag
                            if ((buffer[currentIndex] & 0x1f) == 0x1f)
                            {
                                int endTagIndex = currentIndex;
                                while ((buffer[++endTagIndex] & 0x80) == 0x80) ; //判断第二个字节的最高位是否为1
                                int tagSize = endTagIndex - currentIndex + 1; //计算Tag包含多少字节

                                tlvEntity.Tag = new byte[tagSize];
                                Array.Copy(buffer, currentIndex, tlvEntity.Tag, 0, tagSize);

                                tlvEntity.TagSize = tagSize;

                                currentIndex += tagSize;
                            }
                            else
                            {
                                tlvEntity.Tag = new byte[1];
                                Array.Copy(buffer, currentIndex, tlvEntity.Tag, 0, 1);

                                tlvEntity.TagSize = 1;

                                currentIndex += 1;
                            }
                        }
                        else
                        {
                            //判断是否多字节Tag
                            if ((buffer[currentIndex] & 0x1f) == 0x1f)
                            {
                                int endTagIndex = currentIndex;
                                while ((buffer[++endTagIndex] & 0x80) == 0x80) ; //判断第二个字节的最高位是否为1
                                int tagSize = endTagIndex - currentIndex + 1; //计算Tag包含多少字节

                                tlvEntity.Tag = new byte[tagSize];
                                Array.Copy(buffer, currentIndex, tlvEntity.Tag, 0, tagSize);

                                tlvEntity.TagSize = tagSize;

                                currentIndex += tagSize;
                            }
                            else
                            {
                                tlvEntity.Tag = new byte[1];
                                Array.Copy(buffer, currentIndex, tlvEntity.Tag, 0, 1);

                                tlvEntity.TagSize = 1;

                                currentIndex += 1;
                            }

                            //分析SubTag
                            int subLength = 0;

                            byte[] temp;
                            if ((buffer[currentIndex] & 0x80) == 0x80)
                            {
                                for (int index = 0; index < 2; index++)
                                {
                                    subLength += buffer[currentIndex + 1 + index] << (index * 8); //计算Length域的长度
                                }

                                temp = new byte[subLength];

                                Array.Copy(buffer, currentIndex + 3, temp, 0, subLength);
                            }
                            else
                            {
                                subLength = buffer[currentIndex];

                                temp = new byte[subLength];

                                Array.Copy(buffer, currentIndex + 1, temp, 0, subLength);
                            }

                            tlvEntity.Sub_TLVEntity = new TLVEntity();
                            List<TLVEntity> tempList = Construct(temp);
                            tlvEntity.Sub_TLVEntity = tempList[0];
                        }

                        currentStatus = 'L';
                        break;
                    case 'L':
                        //判断长度字节的最高位是否为1，如果为1，则该字节为长度扩展字节，由下一个字节开始决定长度
                        if ((buffer[currentIndex] & 0x80) != 0x80)
                        {
                            tlvEntity.Length = new byte[1];
                            Array.Copy(buffer, currentIndex, tlvEntity.Length, 0, 1);

                            tlvEntity.LengthSize = 1;

                            valueSize = tlvEntity.Length[0];
                            currentIndex += 1;
                        }
                        else
                        {
                            //为1的情况

                            int lengthSize = buffer[currentIndex] & 0x7f;

                            currentIndex += 1; //从下一个字节开始算Length域

                            for (int index = 0; index < lengthSize; index++)
                            {
                                valueSize += buffer[currentIndex + index] << (index * 8); //计算Length域的长度
                            }

                            tlvEntity.Length = new byte[lengthSize];
                            Array.Copy(buffer, currentIndex, tlvEntity.Length, 0, lengthSize);

                            tlvEntity.LengthSize = lengthSize;

                            currentIndex += lengthSize;
                        }

                        currentStatus = 'V';
                        break;
                    case 'V':
                        tlvEntity.Value = new byte[valueSize];
                        Array.Copy(buffer, currentIndex, tlvEntity.Value, 0, valueSize);

                        currentIndex += valueSize;

                        //进入下一个TLV构造循环
                        list.Add(tlvEntity);

                        currentStatus = 'T';
                        break;
                    default:
                        return new List<TLVEntity>();
                }
            }

            return list;
        }

        /// <summary>
        /// 解析TLV
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static byte[] Parse(List<TLVEntity> list)
        {
            byte[] buffer = new byte[4096];
            int currentIndex = 0;
            int currentTLVIndex = 0;
            int valueSize = 0;

            while (currentTLVIndex < list.Count())
            {
                valueSize = 0;
                TLVEntity entity = list[currentTLVIndex];

                Array.Copy(entity.Tag, 0, buffer, currentIndex, entity.TagSize);	//解析Tag

                currentIndex += entity.TagSize;

                for (int index = 0; index < entity.LengthSize; index++)
                {
                    valueSize += entity.Length[index] << (index * 8); //计算Length域的长度
                }
                if (valueSize > 127)
                {
                    buffer[currentIndex] = Convert.ToByte(0x80 | entity.LengthSize);
                    currentIndex += 1;
                }

                Array.Copy(entity.Length, 0, buffer, currentIndex, entity.LengthSize);	//解析Length

                currentIndex += entity.LengthSize;
                //判断是否包含子嵌套TLV
                if (entity.Sub_TLVEntity == null)
                {
                    Array.Copy(entity.Value, 0, buffer, currentIndex, valueSize); 	//解析Value
                    currentIndex += valueSize;
                }
                else
                {
                    byte[] tempBuffer = Parse(new List<TLVEntity> { entity.Sub_TLVEntity });
                    Array.Copy(tempBuffer, 0, buffer, currentIndex, tempBuffer.Length);	//解析子嵌套TLV
                    currentIndex += tempBuffer.Length;
                }

                currentTLVIndex++;
            }

            byte[] resultBuffer = new byte[currentIndex];
            Array.Copy(buffer, 0, resultBuffer, 0, currentIndex);

            return resultBuffer;
        }
    }
}
