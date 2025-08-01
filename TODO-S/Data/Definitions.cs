using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TODO_S.Data
{
    //##################
    //# Static strings #
    //##################

    public static class StCMessages
    {
        public static readonly string MESSAGE_NOITEMS = "No items stored in fake server.";
        public static readonly string MESSAGE_BADBODY = "Body format or missing body issue.";
        public static readonly string MESSAGE_BADKEY = "Invalid key for fetching item.";
        public static readonly string MESSAGE_SUCCESS = "Success.";
        public static readonly string MESSAGE_SERVERSUCKS = "Unexpected error occurred.";
    }


    //######################
    //# Object Definitions #
    //######################

    public class ListItem
    {
        [Required] public string Label { get; set; }
        [Required] public DateTime DueDate { get; set; }
        [Required(AllowEmptyStrings = true)] public string Description { get; set; }
        [Required] public bool IsCompleted { get; set; }

        public ListItem() 
        {
            Label = string.Empty;
            DueDate = DateTime.Now;
            Description = string.Empty;
            IsCompleted = false;
        }

        public ListItem(string label, DateTime dueDate, string description)
        {
            Label = label;
            DueDate = dueDate;
            Description = description;
            IsCompleted = false;
        }

        public void Clone(ListItem incomming)
        {
            Label = incomming.Label;
            DueDate = incomming.DueDate;
            Description = incomming.Description;
            IsCompleted = incomming.IsCompleted;
        }
    }

    //##########################
    //# HTTP Response Handling #
    //##########################

    public class Response
    {
        public string Message { get; set; }
        public Response()
        {
            Message = string.Empty;
        }
        public Response(string message)
        {
            Message = message;
        }

        public virtual string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class CreateResponse : Response
    {
        public string Key { get; set; }

        public CreateResponse() 
        { 
            Key = string.Empty;
        }

        public CreateResponse(string key, string message) : base(message)
        {
            Key = key;
        }

        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class GetAllResponse : Response
    {
        public Dictionary<string, ListItem> AllItems { get; set; } = new Dictionary<string, ListItem>();

        public GetAllResponse() { }

        public GetAllResponse(Dictionary<string, ListItem> allItems, string message) : base(message) 
        {
            AllItems = allItems;
        }

        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class GetSingleResponse : Response
    {
        public ListItem Item { get; set; }

        public GetSingleResponse() 
        {
            Item = new ListItem();  
        }

        public GetSingleResponse(ListItem item, string message) : base(message)
        {
            Item = item;
        }

        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    //#################
    //# Fake Database #
    //#################

    public interface IListItemDatabase
    {
        Dictionary<string, ListItem> GetAll();
        void Add(string key, ListItem item);
        bool TryGet(string key, out ListItem item);
        int Count();
    }

    public class ListItemDatabase : IListItemDatabase
    {
        private readonly Dictionary<string, ListItem> m_fakeDatabase = new Dictionary<string, ListItem>();

        public Dictionary<string, ListItem> GetAll() 
        {
            return m_fakeDatabase;
        }

        public void Add(string key, ListItem item)
        {
            m_fakeDatabase[key] = item;
        }

        public bool TryGet(string key, out ListItem item)
        {
            return m_fakeDatabase.TryGetValue(key, out item);
        }

        public int Count()
        {
            return m_fakeDatabase.Count;
        }
    }
}
