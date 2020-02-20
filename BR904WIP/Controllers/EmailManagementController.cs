using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BR904WIP.Helpers;
using BR904WIP.Models.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BR904WIP.Controllers
{
    [Produces("application/json")]
    [Route("wipapi")]
    public class EmailManagementController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        public EmailManagementController(IConfiguration iconfiguration)
        {
            _dbHelper = new DatabaseHelper();
        }
        /// <summary>
        /// Create ProcessEmail
        /// </summary>
        /// <param name="emailobj">ProcessEmail</param>
        /// <param name="api_key">api_key</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create904")]
        public IActionResult CreateProcessEmail([FromBody]Email emailobj, string api_key = null)
        {
            if (!string.IsNullOrEmpty(api_key))//check if api_key is NOT Null.
            {
                //Verify the Required Fields
                if (emailobj.process_status == null || emailobj.submission_type == null || emailobj.submitter_email == null
                    || emailobj.create_datetime == null || emailobj.create_user_id == null || emailobj.edit_datetime == null || emailobj.edit_user_id == null)
                {
                    return BadRequest(error: "Request is missing required fields.");
                }

                try
                {
                    using (var cmd = _dbHelper.SpawnCommand())
                    {
                        string createuserId = emailobj.create_user_id ?? Guid.NewGuid().ToString();
                        string edituserId = emailobj.edit_user_id ?? Guid.NewGuid().ToString();
                        string submissionid = emailobj.submission_id ?? Guid.NewGuid().ToString();
                        var timestamp = DateTime.UtcNow;
                        string command = string.Format(@"INSERT INTO public.""904-ProcessEmail""(create_datetime, edit_datetime, process_attempts, process_status, source_sys_type_id, source_sys_type_name, submission_id, submission_type, submitter_email, create_user_id, edit_user_id)
                                                         VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');",
                                                         emailobj.create_datetime ?? timestamp, emailobj.edit_datetime ?? timestamp, emailobj.process_attempts ?? 0, emailobj.process_status ?? string.Empty, emailobj.source_system_type_id ?? string.Empty,
                                                         emailobj.source_sys_type_name ?? string.Empty, submissionid, emailobj.submission_type ?? string.Empty, emailobj.submitter_email ?? string.Empty, createuserId, edituserId);
                        cmd.CommandText = command;
                        int count = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Succsess", statuscode = 200, submission_id = submissionid });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(error: ex.Message);
                }
                finally
                {
                    _dbHelper.CloseConnection();
                }

            }
            return BadRequest(new { message = "API key is missing.", statuscode = 403 });
        }

        [HttpPost]
        [Route("Update904")]
        public IActionResult UpdateProcessEmail([FromBody]Email emailobj, string api_key = null)
        {
            if (!string.IsNullOrEmpty(api_key))//check if api_key is NOT Null.
            {
                try
                {
                    // Verify the Required Fields
                    if (emailobj.process_status == null || emailobj.submission_type == null || emailobj.submitter_email == null
                        || emailobj.create_datetime == null || emailobj.create_user_id == null || emailobj.edit_datetime == null || emailobj.edit_user_id == null)
                    {
                        return BadRequest(error: "Request is missing required fields.");
                    }
                    using (var cmd = _dbHelper.SpawnCommand())
                    {
                        string edituserId = emailobj.edit_user_id ?? Guid.NewGuid().ToString();
                        string submissionid = emailobj.submission_id;
                        var timestamp = DateTime.UtcNow;
                        if (!string.IsNullOrEmpty(submissionid))
                        {

                            string command = string.Format(@"UPDATE public.""904-ProcessEmail"" SET edit_datetime='{0}',process_status ='{1}', source_sys_type_id ='{2}', 
                                                         source_sys_type_name ='{3}', submission_type ='{4}', submitter_email ='{5}'
                                                         WHERE submission_id='{6}';",
                                                             emailobj.edit_datetime ?? timestamp, emailobj.process_status ?? string.Empty, emailobj.source_system_type_id ?? string.Empty,
                                                             emailobj.source_sys_type_name ?? string.Empty, emailobj.submission_type ?? string.Empty, emailobj.submitter_email ?? string.Empty, submissionid);
                            cmd.CommandText = command;
                            int affctedrowcount = cmd.ExecuteNonQuery();
                            if (affctedrowcount == 0)
                                return BadRequest(new { message = "No matching record found for submission_id=" + submissionid, statuscode = 204 });
                            else
                                return Ok(new { statuscode = 200, message = "Succsess" });
                        }
                        else
                        {
                            return BadRequest(new { statuscode = "submission_id can't be null" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(error: ex.Message);
                }
                finally
                {
                    _dbHelper.CloseConnection();
                }

            }
            return BadRequest(new { message = "API key is missing.", statuscode = 403 });
        }

        [HttpGet]
        [Route("Find904")]
        public IActionResult GetProcessEmails([FromBody]SerchCriteria serchcriteria)
        {
            try
            {
                List<Dictionary<string, object>> ProcessEmail = new List<Dictionary<string, object>> { };
                string query = "SELECT * FROM public.\"904-ProcessEmail\"";
                string where = string.Empty;
                if (serchcriteria != null)
                {
                    //Search By Process_status
                    if (serchcriteria.Process_status != null)
                        where = string.Format(" WHERE process_status IN({0}) ", "'" + string.Join("','", serchcriteria.Process_status) + "'");

                    //Search By Process_time
                    if (serchcriteria.Process_time.HasValue)
                    {
                        if (string.IsNullOrEmpty(where))
                        {
                            where = string.Format(" WHERE process_status ='{0}' AND edit_datetime<='{1}' ", "processing", serchcriteria.Process_time);
                        }
                        else
                        {
                            where += string.Format(" AND process_status ='{0}' AND edit_datetime<='{1}' ", "processing", serchcriteria.Process_time);
                        }
                    }

                    //Search By Create_datetime
                    if (serchcriteria.Create_datetime.HasValue)
                    {
                        if (string.IsNullOrEmpty(where))
                        {
                            where = string.Format(" WHERE CAST(create_datetime AS date) ='{0}'", Convert.ToDateTime(serchcriteria.Create_datetime).ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            where += string.Format(" AND CAST(create_datetime AS date) ='{0}'", Convert.ToDateTime(serchcriteria.Create_datetime).ToString("yyyy-MM-dd"));
                        }
                    }

                    //TODO: Need Confirmation on this filter from **Doug**.
                    //Search By Customer_id
                    //if (!string.IsNullOrEmpty(serchcriteria.Customer_id))
                    //{
                    //    if (string.IsNullOrEmpty(where))
                    //    {
                    //        where = string.Format(" WHERE create_user_id ='{0}'", serchcriteria.Customer_id);
                    //    }
                    //    else
                    //    {
                    //        where += string.Format(" AND create_user_id ='{0}'", serchcriteria.Customer_id);
                    //    }
                    //}

                    query += where;
                    using (var cmd = _dbHelper.SpawnCommand())
                    {
                        cmd.CommandText = query;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProcessEmail.Add(new Dictionary<string, object>
                            {
                                { "create_datetime", reader[0] },
                                { "edit_datetime", reader[1] },
                                { "process_attempts",reader[2] },
                                { "process_status", reader[3] },
                                { "source_sys_type_id", reader[4] },
                                { "source_sys_type_name", reader[5] },
                                { "submission_id", reader[6] },
                                { "submission_type", reader[7] },
                                { "submitter_email", reader[8] },
                                { "create_user_id", reader[9] },//For Admin Response
                                { "edit_user_id", reader[10] },//For Admin Response
                            });
                            }
                        }
                    }
                }
                if (ProcessEmail.Count == 0)
                {
                    return BadRequest(new { message = "No Record found!", statuscode = 204 });
                }
                else
                {
                    if (!string.IsNullOrEmpty(serchcriteria.detail_level))//Insure level of response
                    {
                        serchcriteria.detail_level = serchcriteria.detail_level.ToLower();
                        switch (serchcriteria.detail_level)
                        {
                            case "basic":
                                foreach (var item in ProcessEmail) { item.Remove("create_user_id"); item.Remove("edit_user_id"); };
                                break;
                            case "all":
                                foreach (var item in ProcessEmail) { item.Remove("create_user_id"); item.Remove("edit_user_id"); };
                                break;
                            case "admin":
                                break;
                            default:
                                break;
                        }
                    }
                    else //return basic response if detail_level is not specified.
                    {
                        foreach (var item in ProcessEmail) { item.Remove("create_user_id"); item.Remove("edit_user_id"); };
                    }

                    return Ok(new { statuscode = 200, data = ProcessEmail });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(error: ex.Message);
            }
            finally
            {
                _dbHelper.CloseConnection();
            }
        }

        [HttpGet]
        [Route("Get904")]
        public IActionResult GetProcessEmailBySubmissionId(string submission_id)
        {
            try
            {
                var ProcessEmail = new List<Dictionary<string, object>> { };
                string query = "SELECT * FROM public.\"904-ProcessEmail\"";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(submission_id))
                {
                    where = string.Format(" WHERE submission_id ='{0}'", submission_id);
                    query += where;
                    using (var cmd = _dbHelper.SpawnCommand())
                    {
                        cmd.CommandText = query;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProcessEmail.Add(new Dictionary<string, object>
                            {
                                { "create_datetime", reader[0] },
                                { "create_user_id", reader[9] },
                                { "edit_datetime", reader[1] },
                                { "edit_user_id", reader[10] },
                                { "process_attempts",reader[2] },
                                { "process_status", reader[3] },
                                { "source_sys_type_id", reader[4] },
                                { "source_sys_type_name", reader[5] },
                                { "submission_id", reader[6] },
                                { "submission_type", reader[7] },
                                { "submitter_email", reader[8] },
                            });
                            }
                        }
                    }
                    if (ProcessEmail.Count == 0)
                    {
                        return BadRequest(new { message = "No Record found!", statuscode = 204 });
                    }
                    return Ok(new { statuscode = 200, data = ProcessEmail });
                }
                else
                {
                    return BadRequest(new { statuscode = "submission_id can't be null" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(error: ex.Message);
            }
            finally
            {
                _dbHelper.CloseConnection();
            }
        }

        [HttpPost]
        [Route("Remove904")]
        public IActionResult RemoveProcessEmail(string submission_id)
        {
            try
            {
                string query = "Delete FROM public.\"904-ProcessEmail\"";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(submission_id))
                {
                    where = string.Format(" WHERE submission_id ='{0}'", submission_id);
                    query += where;
                    using (var cmd = _dbHelper.SpawnCommand())
                    {
                        cmd.CommandText = query;
                        int deletedCount = cmd.ExecuteNonQuery();
                        if (deletedCount == 0)
                            return BadRequest(new { message = "No matching record found for submission_id = " + submission_id, statuscode = 204 });
                        else
                            return Ok(new { message = $"Deleted {deletedCount} record(s)", statuscode = 200 });
                    }
                }
                else
                {
                    return BadRequest(new { message = "submission_id can't be null" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(error: ex.Message);
            }
            finally
            {
                _dbHelper.CloseConnection();
            }
        }
    }
}