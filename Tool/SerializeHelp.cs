using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace Tool
{
    public static class SerializeHelp
    {
        public static byte[] Serialize<T>(T t)
        {
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(mStream, t);
            return mStream.GetBuffer();
        }
        public static byte[] SerializeList<T>(List<T> t)
        {
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(mStream, t);
            return mStream.GetBuffer();
        }
        public static T Deserialize<T>(byte[] b)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            return (T)bFormatter.Deserialize(new MemoryStream(b));
        }
        public static List<T> DeserializeList<T>(byte[] b)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            return (List<T>)bFormatter.Deserialize(new MemoryStream(b));
        }

        public static void SerializeToFile<T>(T t, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, t);
            }
        }
        public static T DeserializeFromFile<T>(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(fs);
            }
        }
    }
}
