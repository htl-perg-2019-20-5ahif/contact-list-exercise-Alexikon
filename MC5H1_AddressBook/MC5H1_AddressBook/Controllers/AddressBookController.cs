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
        public static List<AddressPage> addressBook { get; set; } = new List<AddressPage> { };

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
        public IActionResult GetPage()
        {
            return Ok(addressBook);
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
        public IActionResult Post([FromBody] AddressPage addressPage)
        { 
            if(ValidateAddressPage(addressPage))
            {
                addressBook.Add(addressPage);

                if(addressPage.FirstName == null)
                {
                    addressPage.FirstName = "";
                }
                if(addressPage.LastName == null)
                {
                    addressPage.LastName = "";
                }

                return Created("Person successfully created", addressPage);
            }

            return BadRequest("Invalid input(e.g.required field missing or empty)");
        }

        public Boolean ValidateAddressPage(AddressPage addressPage)
        {
            if (addressPage.Email != null && addressPage.Email.Length > 0 && addressPage.Email.Contains('@') && addressPage.Id != 0)
            {
                foreach(var addressP in addressBook)
                {
                    if(addressP.Id == addressPage.Id || addressPage.Email.Equals(addressP.Email))
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
                if(addressPage.Id == personId)
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
                if(addressPage.FirstName.Contains(nameFilter) || addressPage.LastName.Contains(nameFilter))
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