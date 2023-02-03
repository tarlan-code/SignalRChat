using ChatAppProject.Models;

namespace ChatAppProject.Data
{
    public static class ClientSource
    {
        public static List<Client> Clients {get;} = new List<Client>();
        public static List<string> Groups { get; } = new List<string>();
    }
}
