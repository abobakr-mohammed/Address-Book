using AutoMapper;
using Azure.Core;
using BusinessTier.DTOS;
using BusinessTier.Repository;
using DataTier.Database;
using DataTier.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AddressBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IGenericRepository<User> _repository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AccountController(IConfiguration config, IGenericRepository<User> repository, IMapper mapper, ApplicationDbContext context)
        {
            _config = config;
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }



        [Route("Register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            try
            {
                var EmailCheck = checkEmail(userDto.Email);
                if (EmailCheck)
                {
                    return new JsonResult("This Email Address Already Exists");
                }
                CreatePasswordHash(userDto.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
                var user = _mapper.Map<User>(userDto);
                user.PasswordHash = PasswordHash;
                user.PasswordSalt = PasswordSalt;
                _repository.InsertAsync(user);
                return new JsonResult(user);
            }
            catch (Exception ex)
            {

                return new JsonResult(ex.Message);
            }


        }
        #region Login 
        [Route("Login")]
        [AllowAnonymous]
        [HttpPost]

        public IActionResult Login([FromBody] UserDto userDto)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(d => d.Email.ToLower() == userDto.Email.ToLower());
                if (user.Email != userDto.Email)
                {
                    return new JsonResult("User Not Found");
                }
                if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return new JsonResult("Wrong Password");
                }

                string token = CreateToken(user);

                return Ok(token);
            }
            catch (Exception ex)
            {

                return new JsonResult(ex.Message);
            }


        }
        #endregion

        #region Ckeck Email
        private bool checkEmail(string EmailAddress)
        {
            var UserEmail = _context.Users.FirstOrDefault(w => w.Email.ToLower() == EmailAddress.ToLower());
            if (UserEmail == null)
                return false;
            else
                return true;
        }
        #endregion

        #region Password Hashing
        private void CreatePasswordHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using(var hmac = new HMACSHA512())
                {
                     PasswordSalt = hmac.Key;
                     PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                }
        }
        #endregion

        #region Verify Password

        private bool VerifyPasswordHash(string Password,  byte[]PasswordHash,  byte[] PasswordSalt)
        {
            using(var hmac = new HMACSHA512(PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                return computedHash.SequenceEqual(PasswordHash);
            }
        }

        #endregion

        #region Create Token
        private string CreateToken(User user)
        {

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email)
            };

            var Key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        #endregion
   

    }
}
