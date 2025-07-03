using System;
using System.Threading.Tasks;
using UserManagement.Common.UserModels;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Web.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
        

    [HttpGet]
    [Route("")]
    [Route("active")]
    [Route("notactive")]
    public async Task<ViewResult> List(string? filter)
    {
        if (filter == null)
        {
            var output =  await _userService.GetAllAsync<UserModel>();
            return View(output);
        }
       else
        {
            var statusForFiltering = filter == "active" ? true : false;
            var output = await _userService.GetByActiveStatus(statusForFiltering);
        
            return View(output);
        }

       
    }

    [HttpGet]
    [Route("add")]
    public ViewResult Add()
    {
        return View();
    }

    [HttpGet]
    [Route("view")]
    public async Task<ViewResult> View(long id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return View(user);
    }

    [HttpGet]
    [Route("update")]
    public async Task<ViewResult> Update(long id)
    {
        var user = await _userService.GetByIdAsync<UpdateUserModel>(id);
        return View(user);
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update(UpdateUserModel updateUserMoel)
    {
        if (ModelState.IsValid)
        {
            await _userService.UpdateAsync(updateUserMoel);
            return RedirectToAction("List", "Users");

        }

        return View(updateUserMoel);
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(AddUserModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Add", model);
        }

        try
        {
            await _userService.AddAsync(model);
            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction("List", "Users");
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "An error occurred while creating the user. Please try again.");
            return View("Add", model);
        }
    }

    [HttpGet]
    [Route("Logs")]
    public async Task<ViewResult> Logs(long id)
    {
        var user = await _userService.GetUserAndLogsAsync(id);
        return View(user);
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<RedirectToActionResult> Delete(long id)
    {
        await _userService.DeleteAsync(id);
        
        return RedirectToAction("List", "Users");
    }


}
