//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System.Runtime.Serialization;
using UnityEngine;

namespace HTC.UnityPlugin.ViveShare.Surrogates
{
    sealed class Vector2SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {

            Vector2 vec = (Vector2)obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
        }

        // method for object deserialization
        public System.Object SetObjectData(object obj,
                                           SerializationInfo info, StreamingContext context,
                                           ISurrogateSelector selector)
        {

            Vector2 vec = (Vector2)obj;
            vec.x = (float)info.GetValue("x", typeof(float));
            vec.y = (float)info.GetValue("y", typeof(float));
            obj = vec;
            return obj;
        }
    }

    sealed class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {

            Vector3 v3 = (Vector3)obj;
            info.AddValue("x", v3.x);
            info.AddValue("y", v3.y);
            info.AddValue("z", v3.z);
        }

        // method for object deserialization
        public System.Object SetObjectData(System.Object obj,
                                           SerializationInfo info, StreamingContext context,
                                           ISurrogateSelector selector)
        {

            Vector3 v3 = (Vector3)obj;
            v3.x = (float)info.GetValue("x", typeof(float));
            v3.y = (float)info.GetValue("y", typeof(float));
            v3.z = (float)info.GetValue("z", typeof(float));
            obj = v3;
            return obj;
        }
    }

    sealed class Vector4SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {

            Vector4 vec = (Vector4)obj;
            info.AddValue("w", vec.w);
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
            info.AddValue("z", vec.z);
        }

        // method for object deserialization
        public System.Object SetObjectData(object obj,
                                           SerializationInfo info, StreamingContext context,
                                           ISurrogateSelector selector)
        {

            Vector4 vec = (Vector4)obj;
            vec.x = (float)info.GetValue("x", typeof(float));
            vec.y = (float)info.GetValue("y", typeof(float));
            vec.z = (float)info.GetValue("z", typeof(float));
            vec.w = (float)info.GetValue("w", typeof(float));
            obj = vec;
            return obj;
        }
    }

    sealed class QuaternionSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {

            Quaternion quat = (Quaternion)obj;
            info.AddValue("x", quat.x);
            info.AddValue("y", quat.y);
            info.AddValue("z", quat.z);
            info.AddValue("w", quat.w);
        }

        // method for object deserialization
        public System.Object SetObjectData(object obj,
                                           SerializationInfo info, StreamingContext context,
                                           ISurrogateSelector selector)
        {

            Quaternion quat = (Quaternion)obj;
            quat.x = (float)info.GetValue("x", typeof(float));
            quat.y = (float)info.GetValue("y", typeof(float));
            quat.z = (float)info.GetValue("z", typeof(float));
            quat.w = (float)info.GetValue("w", typeof(float));
            obj = quat;
            return obj;
        }
    }

    sealed class ColorSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {

            Color color = (Color)obj;
            info.AddValue("r", color.r);
            info.AddValue("g", color.g);
            info.AddValue("b", color.b);
            info.AddValue("a", color.a);
        }

        // method for object deserialization
        public System.Object SetObjectData(object obj,
                                           SerializationInfo info, StreamingContext context,
                                           ISurrogateSelector selector)
        {

            Color color = (Color)obj;
            color.r = (float)info.GetValue("r", typeof(float));
            color.g = (float)info.GetValue("g", typeof(float));
            color.b = (float)info.GetValue("b", typeof(float));
            color.a = (float)info.GetValue("a", typeof(float));
            obj = color;
            return obj;
        }
    }

    sealed class RectSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {

            Rect rect = (Rect)obj;
            info.AddValue("x", rect.x);
            info.AddValue("y", rect.y);
            info.AddValue("w", rect.width);
            info.AddValue("h", rect.height);
        }

        // method for object deserialization
        public System.Object SetObjectData(object obj,
                                           SerializationInfo info, StreamingContext context,
                                           ISurrogateSelector selector)
        {

            Rect rect = (Rect)obj;
            rect.x = (float)info.GetValue("x", typeof(float));
            rect.y = (float)info.GetValue("y", typeof(float));
            rect.width = (float)info.GetValue("w", typeof(float));
            rect.height = (float)info.GetValue("h", typeof(float));
            obj = rect;
            return obj;
        }
    }
}
