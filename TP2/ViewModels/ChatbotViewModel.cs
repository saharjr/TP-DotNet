using System.Collections.Generic;

namespace TP2.ViewModels
{
    public class ChatRequest
    {
        public string Message { get; set; }
    }

    public class ChatResponse
    {
        public string Reply { get; set; }
        public List<string> Suggestions { get; set; } = new List<string>();
    }
}