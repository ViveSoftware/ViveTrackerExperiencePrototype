//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using HTC.UnityPlugin.ViveShare.Surrogates;

public class ViveShare_BinaryFormatter
{
    private static BinaryFormatter m_formatter;

    public static BinaryFormatter binaryFormatter
    {
        get
        {
            if (m_formatter == null)
            {
                m_formatter = new BinaryFormatter();

                SurrogateSelector surrogateSelector = new SurrogateSelector();

                // Vector2
                Vector2SerializationSurrogate vec2_ss = new Vector2SerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), vec2_ss);

                // Vector3
                Vector3SerializationSurrogate vec3_ss = new Vector3SerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vec3_ss);

                // Vector4
                Vector4SerializationSurrogate vec4_ss = new Vector4SerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), vec4_ss);

                // Quaternion
                QuaternionSerializationSurrogate quat_ss = new QuaternionSerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quat_ss);

                // Color
                ColorSerializationSurrogate color_ss = new ColorSerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), color_ss);

                // Rect
                RectSerializationSurrogate rect_ss = new RectSerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(Rect), new StreamingContext(StreamingContextStates.All), rect_ss);

                binaryFormatter.SurrogateSelector = surrogateSelector;
            }

            return m_formatter;
        }
    }
}
