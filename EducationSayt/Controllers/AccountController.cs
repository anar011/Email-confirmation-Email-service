using EducationSayt.Models;
using EducationSayt.Services.Interfaces;
using EducationSayt.ViewModels.Account;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;

namespace EducationSayt.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser newUser = new()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(model);
            }

       
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                                                                                                    //Scheme - Htmp,Htps olamgin gosterir///   Domainin adini gotuub ona birlesdirir

            string link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = newUser.Id, token } , Request.Scheme,Request.Host.ToString());

            string subject = "Register confirmation";

            string html = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/templates/verify.html"))
            {
                html = reader.ReadToEnd();
            }

            html = html.Replace("{{link}}", link);
            html = html.Replace("{{headerText}}", "Hello P135");

            _emailService.Send(newUser.Email,subject,html);

            return RedirectToAction(nameof(VerifyEmail));


            //// create email message                   
            //var email = new MimeMessage(); 
            //email.From.Add(MailboxAddress.Parse("anaraa@code.edu.az"));                          // Hnasi adresden email gedecek
            //email.To.Add(MailboxAddress.Parse(newUser.Email));                                  // Email hara gedecek
            //email.Subject = "Register confirmation";                                           // Email-in basligi
            //email.Body = new TextPart(TextFormat.Html) { Text = $"<a href='{link}'>Go to Elearn</a>" };   // Eamil-neve html getsin?

            //// send email
            //using (var smtp = new SmtpClient())
            //{
            //    smtp.ServerCertificateValidationCallback = (s,c,h,e) => true;
            //    //datalarin tehlukesiz gedib-gelmesi ucun istifade olunur//
            //    smtp.Connect("smtp.gmail.com", 587,SecureSocketOptions.Auto);          //emailin nece,hansi tipde yazilmasi
            //    smtp.Authenticate("anaraa@code.edu.az", "oidrjshchyxnalvd");
            //    smtp.Send(email);                             //emaili-gondermek hissesi
            //    smtp.Disconnect(true);
            //}   




        }

        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if (userId == null || token == null) return BadRequest();

            AppUser user = await _userManager.FindByEmailAsync(userId);

            if (user == null) return NotFound();

            await _userManager.ConfirmEmailAsync(user, token);

            await _signInManager.SignInAsync(user, false);  // Avtomatik user olmaq ucun//

            return RedirectToAction("Index","Home");
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            AppUser user = await _userManager.FindByEmailAsync(model.EmailOrUsername);

            if (user is null)
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUsername);
            }

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }








        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
