using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using TODO_S.Data;

namespace TODO_S.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionalityController : ControllerBase
    {
        public readonly IListItemDatabase m_testDatabase;

        public FunctionalityController(IListItemDatabase database)
        {
            m_testDatabase = database;
        }

        [HttpGet]
        public IActionResult GetAllItems()
        {
            if(m_testDatabase == null)
            {
                return Problem(detail: StCMessages.MESSAGE_SERVERSUCKS, statusCode: 500);
            }

            string message = HandleEmptyDatabaseUtil();
            string response = new GetAllResponse(m_testDatabase.GetAll(), message).ToJson();
            Console.WriteLine(response);
            return Ok(response);
        }

        [HttpGet("{key}")]
        public IActionResult GetItem(string key)
        {
            if (m_testDatabase == null)
            {
                return Problem(detail: StCMessages.MESSAGE_SERVERSUCKS, statusCode: 500);
            }

            string message = HandleEmptyDatabaseUtil();

            if (m_testDatabase.TryGet(key, out var item) && item != null)
            {
                string response = new GetSingleResponse(item, message).ToJson();
                return Ok(response);
            }

            return NotFound(StCMessages.MESSAGE_BADKEY);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ListItem item)
        {
            if (item == null)
            {
                return BadRequest(StCMessages.MESSAGE_BADBODY);
            }

            Console.WriteLine($"item received: Label: {item.Label} \n" +
                $"Due Date: {item.DueDate} \n" +
                $"Description: {item.Description}");

            string key = Guid.NewGuid().ToString();
            m_testDatabase.Add(key, item);
            string response = new CreateResponse(key, StCMessages.MESSAGE_SUCCESS).ToJson();
            return Ok(response);
        }

        [HttpDelete("{key}")]
        public IActionResult DeleteItem(string key)
        {
            if (m_testDatabase == null)
            {
                return Problem(detail: StCMessages.MESSAGE_SERVERSUCKS, statusCode: 500);
            }

            string message = HandleEmptyDatabaseUtil();

            if (m_testDatabase.TryGet(key, out var item))
            {
                m_testDatabase.GetAll().Remove(key);
                string response = new GetAllResponse(m_testDatabase.GetAll(), message).ToJson();
                return Ok(response);
            }

            return BadRequest(StCMessages.MESSAGE_BADKEY);
        }

        private string HandleEmptyDatabaseUtil()
        {
            if (m_testDatabase.Count() == 0)
            {
                return StCMessages.MESSAGE_NOITEMS;
            }
            return StCMessages.MESSAGE_SUCCESS;
        }
    }
}
