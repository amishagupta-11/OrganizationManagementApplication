using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationManagementApplication.Data;
using OrganizationManagementApplication.Models;
using OrganizationManagementApplication.Services;
using System.Reflection;

namespace OrganizationManagementApplication.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly OrganizationManagementContext _context;
        private readonly IMapper _mapper;
        private readonly ReflectionSerializationService _serializationService;
        private readonly ReflectionHelperService _reflectionHelperService;

        public EmployeeController(OrganizationManagementContext context, IMapper mapper, ReflectionSerializationService serializationService, ReflectionHelperService reflectionHelperService)
        {
            _context = context;
            _mapper = mapper;
            _serializationService = serializationService;
            _reflectionHelperService = reflectionHelperService;
        }

        /// <summary>
        /// Get endpoint to fetch data from database by mapping the employee with employeeDetails.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.ToListAsync();
            var employeeDetails = _mapper.Map<List<EmployeeDetails>>(employees);
            return View(employeeDetails);
        }

        /// <summary>
        /// Endpoint to display the Add employee form.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Endpoint to post the data filled in the form into the database.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,DateOfBirth,Email")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Create a new Employee entity
                var newEmployee = new Employee();

                // Copy properties from the model to the new entity using reflection
                foreach (var property in typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var newValue = _reflectionHelperService.GetProperty(employee, property.Name);
                    _reflectionHelperService.SetProperty(newEmployee, property.Name, newValue);
                }

                // Add the new employee to the context
                _context.Employees.Add(newEmployee);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        /// <summary>
        /// Enpoint to check whether requested id is present in the database or not.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        /// <summary>
        /// Endpoint to update the data in the database using reflections
        /// </summary>
        /// <param name="id"></param>
        /// <param name="employee"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,DateOfBirth,Email")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Use reflection to update properties dynamically
                    var existingEmployee = await _context.Employees.FindAsync(id);
                    if (existingEmployee == null) return NotFound();

                    foreach (var property in typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var newValue = _reflectionHelperService.GetProperty(employee, property.Name);
                        _reflectionHelperService.SetProperty(existingEmployee, property.Name, newValue);
                    }

                    _context.Update(existingEmployee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }



        /// <summary>
        /// endpoint to check whether requested id is present in the database or not.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        /// <summary>
        /// Endpoint to delete the data related to the requested id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }


        /// <summary>
        /// Method to serialize and deserialize the incoming JSON/XML data 
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SerializeEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is null.");
            }

            // Serialize the employee to JSON
            var json = _serializationService.SerializeToJson(employee);

            // Serialize the employee to XML
            var xmlObject = _serializationService.SerializeToXml(employee);

            // Deserialize the JSON back to an Employee object
            var deserializedEmployeeFromJson = _serializationService.DeserializeFromJson(json);

            // Deserialize the XML back to an Employee object
            var deserializedEmployeeFromXml = _serializationService.DeserializeFromXml(xmlObject);

            // Check if deserialization was successful for JSON
            if (deserializedEmployeeFromJson == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "JSON deserialization failed.");
            }

            // Check if deserialization was successful for XML
            if (deserializedEmployeeFromXml == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "XML deserialization failed.");
            }

            var isJsonDeserializationSuccessful = AreEmployeesEqual(employee, deserializedEmployeeFromJson);

            if (!isJsonDeserializationSuccessful)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Deserialized JSON employee does not match the original.");
            }

            var isXmlDeserializationSuccessful = AreEmployeesEqual(employee, deserializedEmployeeFromXml);

            if (!isXmlDeserializationSuccessful)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Deserialized XML employee does not match the original.");
            }

            // Create an object with both serialized and deserialized data
            var result = new
            {
                OriginalEmployee = employee,
                SerializedJson = json,
                SerializedXml = xmlObject,
                DeserializedEmployeeFromJson = deserializedEmployeeFromJson,
                DeserializedEmployeeFromXml = deserializedEmployeeFromXml
            };

            return Ok(result);
        }

        /// <summary>
        /// Helper method to compare two Employee objects
        /// </summary>
        /// <param name="emp1"></param>
        /// <param name="emp2"></param>
        /// <returns></returns>
        private bool AreEmployeesEqual(Employee emp1, Employee emp2)
        {
            if (emp1 == null || emp2 == null)
            {
                return false;
            }

            // Compare each property
            foreach (var property in typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value1 = property.GetValue(emp1);
                var value2 = property.GetValue(emp2);

                // Use equality checks based on property type
                if (!object.Equals(value1, value2))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
