﻿using KasundiRestaurant.Models;
using KasundiRestaurant.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KasundiRestaurant.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Name { get; set; }

            public string StreetAddress { get; set; }
            public string PhoneNumber { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            string role = Request.Form["rdUserRole"].ToString();

            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    City = Input.City,
                    State = Input.State,
                    StreetAddress = Input.StreetAddress,
                    PhoneNumber = Input.PhoneNumber,
                    Name = Input.Name,
                    PostalCode = Input.PostalCode,

                };


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    //new roles
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.ManagerUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.ManagerUser));
                    }

                    if (!await _roleManager.RoleExistsAsync(StaticDetails.CustomerEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.CustomerEndUser));
                    }

                    if (!await _roleManager.RoleExistsAsync(StaticDetails.KitchenUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.KitchenUser));
                    }

                    if (!await _roleManager.RoleExistsAsync(StaticDetails.FrontDeskUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.FrontDeskUser));
                    }

                    //08 march 2023
                    // for admin role
                    //await _userManager.AddToRoleAsync(user, StaticDetails.ManagerUser);
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(returnUrl);

                    //new roles end

                    if (role == StaticDetails.ManagerUser)
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.ManagerUser);
                    }
                    else
                    {
                        if (role == StaticDetails.FrontDeskUser)
                        {
                            await _userManager.AddToRoleAsync(user, StaticDetails.FrontDeskUser);
                        }
                        else
                        {
                            if (role == StaticDetails.KitchenUser)
                            {
                                await _userManager.AddToRoleAsync(user, StaticDetails.KitchenUser);
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, StaticDetails.CustomerEndUser);
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);

                            }
                        }
                    }
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                    //admin

                    _logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    //}
                    //else
                    //{


                    //}
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
