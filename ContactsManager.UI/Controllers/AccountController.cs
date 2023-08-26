using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ContactsManager.UI.Controllers
{
    [Route("/[Controller]")]
    [AllowAnonymous] //Allow unauthorized users also
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signinManager)
        {
            _userManager = userManager;   
            _signinManager = signinManager;
        }

        [Route("/[action]")]
        [HttpGet]        
        public IActionResult Register()
        {
            return View();  
        }

        [HttpPost]
        [Route("/[action]")]
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

        [Route("/[action]")]
        [HttpGet]        
        public IActionResult Login()
        {
            return View();
        }

        [Route("/[action]")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
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
                return RedirectToAction(nameof(PersonsController.Index),"Persons");
            }
            
                ModelState.AddModelError("Login", "Invalid UserName or Password");
                return View(loginDTO);            
            
            
        }

        [Route("/[action]")]      
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

    }
}
