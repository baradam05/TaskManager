using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Classes;
using TaskManager.Models.DbClasses;
using TaskManager.Models.Dto;


namespace TaskManager.Controllers
{
    public class AccountController : Controller
    {
        private MyContext context = new MyContext();

#region Login
        [HttpGet]
        public IActionResult Login()
        {
            var accs = this.context.Account.ToList();

            this.ViewBag.InvalidUsername = false;

            LoginDto login = new LoginDto();
            return View(login);
        }

        [HttpPost]
        public IActionResult Login(LoginDto loginDto)
        {
            Account account = context.Account.Where(x => x.Username.ToLower() == loginDto.Username.ToLower() || x.Email.ToLower() == loginDto.Username.ToLower()).FirstOrDefault();
            if (account == null)
            {
                this.ViewBag.InvalidUsername = true;
                return View();
            }
            else 
            {
                try
                {
                    if (PasswordDecoder.Decode(account.PasswordHash) != loginDto.Password)
                    {
                        this.ViewBag.InvalidUsername = true;
                        return View();
                    }
                }
                catch (Exception)
                {
                    this.ViewBag.InvalidUsername = true;
                    return View();
                }
                
            }

            this.HttpContext.Session.SetString("login", account.AccountId.ToString());

            return RedirectToAction("Index", "Home");
        }
        #endregion
#region Register
        [HttpGet]
        public IActionResult Register()
        {

            this.ViewBag.Leaders = this.context.Account.Where(x => x.LeaderId == null).ToList();

            this.ViewBag.InvalidUsername = false;
            this.ViewBag.InvalidEmail = false;
            this.ViewBag.InvalidPassword = false;
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDto registerDto)
        {
            this.ViewBag.Leaders = this.context.Account.Where(x => x.LeaderId == null).ToList();
            this.ViewBag.InvalidUsername = false;
            this.ViewBag.InvalidEmail = false;
            this.ViewBag.InvalidPassword = false;

            Account? account = context.Account.Where(x => x.Username == registerDto.Username).FirstOrDefault();
            if (account != null)
            {
                this.ViewBag.InvalidUsername = true;
                return View();
            }

            if (registerDto.Email != null)
            {
                account = null;
                account = context.Account.Where(x => x.Email == registerDto.Email).FirstOrDefault();

                if (account != null)
                {
                    this.ViewBag.InvalidEmail = true;
                    return View();
                }
            }

            if (registerDto.Password != registerDto.PasswordAgain) 
            {
                this.ViewBag.InvalidPassword = true;
                return View();
            }

            Account newAccount = new();
            newAccount.LeaderId = registerDto.LeaderId;
            newAccount.Email = registerDto.Email.ToLower();
            newAccount.Username = registerDto.Username.ToLower();
            newAccount.PasswordHash = PasswordDecoder.Encode(registerDto.Password);
            
            this.context.Account.Add(newAccount);
            this.context.SaveChanges();

            this.HttpContext.Session.SetString("login", newAccount.AccountId.ToString());
           
            return RedirectToAction("Login");
        }
        #endregion
        #region Manage
        [Secured]
        [HttpGet]
        public IActionResult Manage()
        {            
            Account loggedIn = this.context.Account.Find(int.Parse(this.HttpContext.Session.GetString("login")));
            
            AccountDto accountDto = new();
            this.ViewBag.isLeader = false;
            if (loggedIn.LeaderId != null) 
            {
                this.ViewBag.isLeader = true;
                accountDto.Leader = context.Account.Find(loggedIn.LeaderId).Username;
            }
            accountDto.Username = loggedIn.Username;
            accountDto.Password = PasswordDecoder.Decode(loggedIn.PasswordHash);
            accountDto.Email = loggedIn.Email;

            this.ViewBag.Success = false;
            return View(accountDto);
        }

        [Secured]
        [HttpPost]
        public IActionResult Manage(AccountDto accountDto)
        {
            Account loggedIn = this.context.Account.Find(int.Parse(this.HttpContext.Session.GetString("login")));

            this.ViewBag.isLeader = false;
            if (loggedIn.LeaderId != null)
                this.ViewBag.isLeader = true;
            
            loggedIn.Username = accountDto.Username;
            loggedIn.PasswordHash = PasswordDecoder.Encode(accountDto.Password);
            loggedIn.Email = accountDto.Email;
            this.context.SaveChanges();

            this.ViewBag.Success = true;
            return View(accountDto);
        }
#endregion

        public IActionResult Logout()
        {
            this.HttpContext.Session.Remove("login");
            return RedirectToAction("Login");
        }
    }
}
