using System;
using System.IO;

enum PacketId : int
{
    CharaterStatReqId = 1,
    CharaterStatResId = 2,
    ChatchReqId = 3,
    ChatchResId = 4
};

public class PacketBase
{
    protected int packetId;
    protected int packetLength;
    public byte[] Serialize()
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(packetId);
            return stream.ToArray();
        }
    }

    public int Deserialize(byte[] bytes)
    {
        int offset = 0;
        packetId = BitConverter.ToInt32(bytes, offset);
        offset += sizeof(int);
        return offset;
    }

    public int GetPacketLength()
    {
        return packetLength;
    }
}

public class CharaterStatReq : PacketBase
{
    public float locX;
    public float locY;
    public float velX;
    public float velY;
    public float sendReqCliTime;
    public float ttsThis;
    public float ttsOther;
    public CharaterStatReq(float inputLocX, float inputLocY, 
        float inputVelX, float inputVelY, float inputSendReqCliTime, 
        float inputTtsThis, float inputTtsOther)
    {
        base.packetId = (int)PacketId.CharaterStatReqId;
        locX = inputLocX;
        locY = inputLocY;
        velX = inputVelX;
        velY = inputVelY;
        sendReqCliTime = inputSendReqCliTime;
        ttsThis = inputTtsThis;
        ttsOther = inputTtsOther;
        base.packetLength = sizeof(int)  + sizeof(float) * 7;
    }

    public new byte[] Serialize()
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            // 부모클래스의 데이터를 먼저 쓰기
            writer.Write(base.Serialize());

            writer.Write(locX);
            writer.Write(locY);
            writer.Write(velX);
            writer.Write(velY);
            writer.Write(sendReqCliTime);
            writer.Write(ttsThis);
            writer.Write(ttsOther);

            return stream.ToArray();
        }
    }
}

public class CharaterStatRes : PacketBase
{
    public float locX;
    public float locY;
    public float velX;
    public float velY;
    public float ttsThis;
    public float ttsOther;

    public new byte[] Serialize()
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(base.Serialize());

            writer.Write(locX);
            writer.Write(locY);
            writer.Write(velX);
            writer.Write(velY);
            writer.Write(ttsThis);
            writer.Write(ttsOther);

            return stream.ToArray();
        }
    }

    public new int Deserialize(byte[] bytes)
    {
        base.packetLength = sizeof(int) + sizeof(float) * 6;

        int offset = base.Deserialize(bytes);
        locX = BitConverter.ToSingle(bytes, offset);
        offset += sizeof(float);
        locY = BitConverter.ToSingle(bytes, offset);
        offset += sizeof(float);
        velX = BitConverter.ToSingle(bytes, offset);
        offset += sizeof(float);
        velY = BitConverter.ToSingle(bytes, offset);
        offset += sizeof(float);
        ttsThis = BitConverter.ToSingle(bytes, offset);
        offset += sizeof(float);
        ttsOther = BitConverter.ToSingle(bytes, offset);
        offset += sizeof(float);

        return offset;
    }

}