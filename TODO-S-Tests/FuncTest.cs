using Microsoft.AspNetCore.Mvc;
using TODO_S.Controllers;
using TODO_S.Data;
using System.Text.Json;

namespace TODO_S_Tests
{
    public class FuncTest
    {
        [Fact]
        public void TestFunctionalityController()
        {
            //Init
            IListItemDatabase testDatabase = new ListItemDatabase();
            FunctionalityController itemFuncController = new FunctionalityController(testDatabase);

            ListItem UtilityCreateListItem(string actualLabel, string actualDesc)
            {
                return new ListItem()
                {
                    Label = actualLabel,
                    Description = actualDesc,
                    DueDate = DateTime.Now,
                };
            }

            ListItem item = UtilityCreateListItem("Foo", "Bar");
            ListItem item2 = UtilityCreateListItem("Foo1", "Bar1");

            IActionResult returnedAll;
            OkObjectResult returnedOk;
            string returnedJson;

            //Begin - No Key

            void UtilityCreateItem(ListItem item)
            {
                IActionResult create = itemFuncController.Create(item);
                returnedOk = Assert.IsType<OkObjectResult>(create);
                returnedJson = Assert.IsType<string>(returnedOk.Value);
                CreateResponse returnedCreateDeser = Assert.IsType<CreateResponse>(JsonSerializer.Deserialize<CreateResponse>(returnedJson));
                string message = returnedCreateDeser.Message;
                Assert.Equal(StCMessages.MESSAGE_SUCCESS, message);
            }

            UtilityCreateItem(item);
            UtilityCreateItem(item2);

            //Begin - Key

            testDatabase = new ListItemDatabase();
            itemFuncController = new FunctionalityController(testDatabase);
            itemFuncController.m_testDatabase.Add("test1", item);
            itemFuncController.m_testDatabase.Add("test2", item2);

            int itemsCountForTest = itemFuncController.m_testDatabase.Count();

            returnedAll = itemFuncController.GetAllItems();
            returnedOk = Assert.IsType<OkObjectResult>(returnedAll);
            returnedJson = Assert.IsType<string>(returnedOk.Value);
            GetAllResponse returnedDictDeser = Assert.IsType<GetAllResponse>(JsonSerializer.Deserialize<GetAllResponse>(returnedJson));
            Dictionary<string, ListItem> returnedDict = returnedDictDeser.AllItems;
            Assert.Equal(itemsCountForTest, returnedDict.Count);

            void UtilityItemSearch(string key, string expectedLabel, string expectedDesc)
            {
                IActionResult returnedItem = itemFuncController.GetItem(key);
                returnedOk = Assert.IsType<OkObjectResult>(returnedItem);
                returnedJson = Assert.IsType<string>(returnedOk.Value);
                GetSingleResponse returnedSingleDeser = Assert.IsType<GetSingleResponse>(JsonSerializer.Deserialize<GetSingleResponse>(returnedJson));
                ListItem testItem = returnedSingleDeser.Item;
                Assert.Equal(expectedLabel, testItem.Label);
                Assert.Equal(expectedDesc, testItem.Description);
            }

            UtilityItemSearch("test1", "Foo", "Bar");
            UtilityItemSearch("test2", "Foo1", "Bar1");

            void UtilityItemDelete(string deleteKey, string expectedLabel, string expectedDesc)
            {
                IActionResult delete = itemFuncController.DeleteItem(deleteKey);
                returnedOk = Assert.IsType<OkObjectResult>(delete);
                returnedJson = Assert.IsType<string>(returnedOk.Value);
                returnedDictDeser = Assert.IsType<GetAllResponse>(JsonSerializer.Deserialize<GetAllResponse>(returnedJson));
                returnedDict = returnedDictDeser.AllItems;
                string deleteSuccess = returnedDictDeser.Message;
                Assert.Equal(StCMessages.MESSAGE_SUCCESS, deleteSuccess);
                Assert.Equal(itemsCountForTest - 1, returnedDict.Count);

                ListItem singleItem = returnedDict.Values.ToList()[0];
                Assert.Equal(expectedLabel, singleItem.Label);
                Assert.Equal(expectedDesc, singleItem.Description);
            }

            UtilityItemDelete("test1", "Foo1", "Bar1");
            itemFuncController.m_testDatabase.Add("test1", item);
            UtilityItemDelete("test2", "Foo", "Bar");
        }
    }
}
