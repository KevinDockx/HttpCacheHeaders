using Marvin.Cache.Headers.Interfaces;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Sample.Controllers
{
    /// <summary>
    /// When an item is in the store, it automatically gets updated when a PUT/PATCH request is sent for that item.  
    /// But sometimes, you might want to manually invalidate it (= remove it from the store).  
    /// 
    /// Think, for example, about a list of courses.  Manipulating that might have an effect on related resources. 
    /// If a PUT statement is sent to one course resource that one course will get a new Etag, but the courseS resource 
    /// doesn’t automatically change. If the course you just updated is one of the courses in the returned courses when 
    /// fetching the courses resource, the courses resource is out of date.  
    /// 
    /// Same goes for deleting or creating a course: that, too, might have an effect on the courses resource, or even other
    /// related resources.  
    /// 
    /// In this sample the IValidatorValueInvalidator & IStoreKeyAccessor are used to support scenarios like that.
    /// </summary>
    [Route("api/storemanipulation")] 
    public class StoreManipulationController : Controller
    {
        // The value invalidator is used to mark values from the store for invalidation (= removal)
        private readonly IValidatorValueInvalidator _validatorValueInvalidator;

        // Values are stored by their store key.  These keys often contain a combination of resource 
        // path, query string, some header values (like Accept, Accept-Encoding, ...), ... but HOW
        // these keys are generated is up to you.  By default, the DefaultStoreKeyGenerator is used 
        // for that, but you can override it by creating your own IStoreKeyGenerator implementation. 
        // 
        // In other words: there's no way to easily "guess" what the keys are. You might want to 
        // store them in your application, but if you don't want to do that you can use the 
        // IStoreKeyAccessor to find the keys you need.
        private readonly IStoreKeyAccessor _storeKeyAccessor;

        public StoreManipulationController(
            IValidatorValueInvalidator validatorValueInvalidator,
            IStoreKeyAccessor storeKeyAccessor)
        {          
            _validatorValueInvalidator = validatorValueInvalidator 
                ?? throw new ArgumentNullException(nameof(validatorValueInvalidator));
            _storeKeyAccessor = storeKeyAccessor 
                ?? throw new ArgumentNullException(nameof(storeKeyAccessor));
        }
 
        [HttpGet] 
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }
         
        [HttpGet("{id}")] 
        public async Task<string> Get(int id)
        { 
            return "value";
        } 

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            // code to post omitted

            // remove all items matching part of a resource path

            // 1) find the keys matching a certain string
            await foreach (var key in _storeKeyAccessor.FindByKeyPart("api/storemanipulation"))
            {
                // 2) mark them (often just one) for invalidation
                await _validatorValueInvalidator.MarkForInvalidation(key);
            }

            // note: if you don't want to work with the IAsyncEnumerable, you can
            // install the https://www.nuget.org/packages/System.Linq.Async/ package
            // and call ToListAsync() to cast it to a regular list. 
             
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            // code to update omitted

            // remove items based on the current path

            // 1) find the keys related to the current resource path
            await foreach (var key in _storeKeyAccessor.FindByCurrentResourcePath())
            {  
                // 2) mark them (often just one) for invalidation
                await _validatorValueInvalidator.MarkForInvalidation(key);
            }

            // note: if you don't want to work with the IAsyncEnumerable, you can
            // install the https://www.nuget.org/packages/System.Linq.Async/ package
            // and call ToListAsync() to cast it to a regular list.


            return NoContent();
        }
    }
}
