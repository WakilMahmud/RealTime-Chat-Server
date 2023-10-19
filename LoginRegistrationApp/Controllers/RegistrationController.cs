using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using LoginRegistrationApp.Models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LoginRegistrationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("registration")]
        public string registration(User user)
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ChatAppConn").ToString());
            SqlCommand cmd = new SqlCommand("INSERT INTO Users(Email, UserName) VALUES('" + user.Email + "', '" + user.UserName + "')", conn);

            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();

            Response res = new Response();

            if (i > 0) { 
                res.statusCode = 201;
                res.statusText = "User Registered Successfully";

                return JsonSerializer.Serialize(res);
            }
            else {
                res.statusCode = 417;
                res.statusText = "Expectation Failed"; 
                
                return JsonSerializer.Serialize(res);
            }
        }



        [HttpPost]
        [Route("login")]
        public string login(Login login)
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ChatAppConn").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users WHERE Email = '" + login.Email + "'", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Response res = new Response();

            if (dt.Rows.Count > 0) {
                res.statusCode = 302;
                res.statusText = "Valid User";
                res.userName = (string?)(dt.Rows[0]["UserName"]);
                return JsonSerializer.Serialize(res);
            }
            else
            {
                res.statusCode = 401;
                res.statusText = "Unauthorized";
                return JsonSerializer.Serialize(res);
            }
        }


        [HttpGet]
        [Route("users")]
        public Response GetUsers() 
        {
            List<User> lstUsers = new List<User>();
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ChatAppConn").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Response res = new Response();

            if (dt.Rows.Count > 0)
            {
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    User users = new User();
                    users.Email = Convert.ToString(dt.Rows[i]["Email"]);
                    users.UserName = Convert.ToString(dt.Rows[i]["UserName"]);

                    lstUsers.Add(users);
                }

                if(lstUsers.Count > 0) 
                { 
                    res.statusCode = 200;
                    res.statusText = "Data Found";
                    res.users = lstUsers;
                }
                else
                {
                    res.statusCode = 100;
                    res.statusText = "No Data Found";
                    res.users = null;
                }
                return res;
            }
            else
            {
                res.statusCode = 100;
                res.statusText = "No Data Found";
                res.users = null;
            }

            return res;
        }
    }
}
