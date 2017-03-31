//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;

namespace HTC.UnityPlugin.ViveShare
{
    public class ViveShare_ParamMessage : MessageBase
    {
        private string m_id;
        public string id
        {
            get { return m_id; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    m_id = value;
                }
            }
        }

        public float timeStamp;
        public object[] objects;


        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(id);
            writer.Write(timeStamp);

            if (objects == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(objects.Length);
                for (int i = 0; i < objects.Length; i++)
                {
                    object obj = objects[i];
                    byte[] bytes = ObjectToByteArray(obj);
                    writer.WriteBytesFull(bytes);
                }
            }
        }

        public override void Deserialize(NetworkReader reader)
        {
            id = reader.ReadString();
            timeStamp = reader.ReadSingle();

            int objNum = reader.ReadInt32();
            objects = objNum == 0 ? null : new object[objNum];

            for (int i = 0; i < objNum; i++)
            {
                short len = reader.ReadInt16();
                byte[] bytes = reader.ReadBytes(len);
                objects[i] = ByteArrayToObject(bytes);
            }
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            BinaryFormatter bf = ViveShare_BinaryFormatter.binaryFormatter;
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        private object ByteArrayToObject(byte[] byteArr)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter bf = ViveShare_BinaryFormatter.binaryFormatter;
            memStream.Write(byteArr, 0, byteArr.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = (object)bf.Deserialize(memStream);
            return obj;
        }
    }
}
