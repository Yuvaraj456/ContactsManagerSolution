using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]")]
    //[AllowAnonymous] //Allow unauthorized users also //for comment this allowanonymous for custom Authorize policy 
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signinManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signinManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;   
            _signinManager = signinManager;
            _roleManager = roleManager;
        }

        [Route("[action]")]
        [HttpGet]
        [Authorize("NotAuthorized")]
        public IActionResult Register()
        {
            return View();  
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize("NotAuthorized")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {   //Check validation Error
            if(ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(temp => temp.ErrorMessage);

                return View(registerDTO);
            }
            ApplicationUser user = new ApplicationUser() 
            {
                Email = registerDTO.Email,
                PersonName = registerDTO.PersonName,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.Email,                
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

            if(result.Succeeded)
            {              
                //check the status of the radio button
                if(registerDTO.UserType == Core.Enums.UsertypeOptions.Admin)
                {
                    //Create Admin role
                    if(await _roleManager.FindByNameAsync(UsertypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole() { Name=UsertypeOptions.Admin.ToString() };

                        await _roleManager.CreateAsync(applicationRole);
                    }

                    //Add the new user into the Admin role
                    await _userManager.AddToRoleAsync(user, UsertypeOptions.Admin.ToString());

                }
                else
                {
                    //Create User role
                    if (await _roleManager.FindByNameAsync(UsertypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole() { Name = UsertypeOptions.User.ToString() };

                        await _roleManager.CreateAsync(applicationRole);
                    }

                    //add the new user into User role
                    await _userManager.AddToRoleAsync(user, UsertypeOptions.User.ToString());

                }
                //signin
                 await _signinManager.SignInAsync(user, isPersistent: false); //isPersistent is true, then login is active even though we                                                               close the browser, if false login is deactive once closed browser.

                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach(IdentityError identityError in result.Errors)
                {
                    ModelState.AddModelError("Register", identityError.Description);
                }
                return View(registerDTO);
            }
           
        }

        [Route("[action]")]
        [HttpGet]
        [Authorize("NotAuthorized")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        [Authorize("NotAuthorized")]
        public async Task<IActionResult> Login(LoginDTO loginDTO,string? returnUrl)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(temp => temp.ErrorMessage);

                return View(loginDTO);
            }
            var result = await _signinManager.PasswordSignInAsync(loginDTO.Email,loginDTO.Password, isPersistent:false, lockoutOnFailure:false);//isPersistent is true, then login is active even though we                                                       close the browser, if false login is deactive once closed browser.
                                                            //lockoutOnFailure is true means if the user enter password incorrectly 3 times the user get locked for a while, if false not get locked while login failed
            if(result.Succeeded)
            {
                //Admin
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if(user != null)
                {
                    if(await _userManager.IsInRoleAsync(user,nameof(UsertypeOptions.Admin)))
                    {
                        return RedirectToAction("Index","Home", new { area = "Admin"});
                    }
                }
                if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction(nameof(PersonsController.Index),"Persons");
            }
            
                ModelState.AddModelError("Login", "Invalid UserName or Password");
                return View(loginDTO);            
            
            
        }

        [Route("[action]")]      
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
           ApplicationUser user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return Json(true); //valid email address
            }
            else
            {
                return Json(false); //invalid email adrress
            }           
        }

    }
}
