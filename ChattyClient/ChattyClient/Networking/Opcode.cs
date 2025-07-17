namespace ChattyClient.Networking
{
    public enum Opcode : byte
    {
        Username = 1,
        Message = 2,
        Disconnect = 3
    }
}
