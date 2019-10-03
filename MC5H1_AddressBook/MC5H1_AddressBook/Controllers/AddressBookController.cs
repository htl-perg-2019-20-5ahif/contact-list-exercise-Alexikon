using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MC5H1_AddressBook.Controllers
{
    [ApiController]
    [Route("contacts")]
    public class AddressBookController : ControllerBase
    {
        public static List<AddressPage> addressBook { get; set; } = new List<AddressPage> { new AddressPage(1, "Test", "Testington", "test.testington@test.at") };

        // GET api/values
        // test return:
        //[
        //   {
        //      "id": 0,
        //      "firstName": "string",
        //      "lastName": "string",
        //      "email": "string"
        //   }
        //]
        [HttpGet]
        public ActionResult<string> GetPage()
        {
            return Ok(JsonConvert.SerializeObject(addressBook));
        }


        // I thought I have to make this, but it is not in yaml specification
        // GET contacts/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    if(id >= 0 && id < Startup.addressBook.Count)
        //    {
        //        return Ok(new JavaScriptSerializer().Serialize(Startup.addressBook.ElementAt(id)));
        //    }
        //    return "value";
        //}


        // POST contacts
        [HttpPost]
        public ActionResult Post([FromBody] string json)
        { 
            AddressPage addressPage = JsonConvert.DeserializeObject<AddressPage>(json);

            if(ValidateAddressPage(addressPage))
            {
                addressBook.Add(addressPage);

                if(addressPage.firstName == null)
                {
                    addressPage.firstName = "";
                }
                if(addressPage.lastName == null)
                {
                    addressPage.lastName = "";
                }

                return CreatedAtRoute("Person successfully created", addressPage);
            }

            return BadRequest("Invalid input(e.g.required field missing or empty)");
        }

        public Boolean ValidateAddressPage(AddressPage addressPage)
        {
            if (addressPage.email != null && addressPage.email.Length > 0 && addressPage.email.Contains('@') && addressPage.id != 0)
            {
                foreach(var addressP in addressBook)
                {
                    if(addressP.id == addressPage.id || addressPage.email.Equals(addressP.email))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [HttpDelete]
        [Route("{personId}")]
        public IActionResult DeletePage(int personId)
        {
            if(personId <= 0)
            {
                return BadRequest("Invalid ID supplied");
            }

            foreach(var addressPage in addressBook)
            {
                if(addressPage.id == personId)
                {
                    addressBook.Remove(addressPage);

                    return NoContent();
                }
            }

            return NotFound("Person not found");
        }

        [HttpGet]
        [Route("findByName")]
        public IActionResult findByName([FromQuery(Name = "nameFilter")] string nameFilter)
        {
            List<AddressPage> response = new List<AddressPage>();

            if(nameFilter == null || nameFilter == "")
            {
                return BadRequest("Invalid or missing name");
            }

            foreach (var addressPage in addressBook)
            {
                if(addressPage.firstName.Contains(nameFilter) || addressPage.lastName.Contains(nameFilter))
                {
                    response.Add(addressPage);
                }
            }

            if(response.Count == 0)
            {
                return BadRequest("Invalid or missing name");
            }

            return Ok(JsonConvert.SerializeObject(response));
        }
    }
}