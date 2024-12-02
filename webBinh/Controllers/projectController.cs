using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using webBinh.Models;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Text;

namespace webBinh.Controllers
{
    public class projectController : Controller
    {
        // GET: project
        //string sdt = "0839976113";
        private webNangCaoQLDA2Entities1 db = new webNangCaoQLDA2Entities1();
        List<webBinh.Models.Task> task = new List<webBinh.Models.Task>();
        List<webBinh.Models.Project> project = new List<webBinh.Models.Project>();
     

        public ActionResult Index(string sdt)
        {
            sdt = "0839976113";
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("http://localhost:5224/GetTenProject?sdt="+sdt).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    project = JsonConvert.DeserializeObject<List<Project>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            return View(project.ToList());
        }
        
        public  ActionResult column(int id_project, string sdt)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("http://localhost:5224/GetTenProject?sdt=" + sdt).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    project = JsonConvert.DeserializeObject<List<Project>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            ViewBag.project = project.ToList();

            var ten= project.Where(i=>i.id_project==id_project).FirstOrDefault();
            ViewBag.ten = ten.title;
            ViewBag.id=id_project;


            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("http://localhost:5224/GetTask?sdt=" + sdt).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    task = JsonConvert.DeserializeObject<List<webBinh.Models.Task>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            ViewBag.task=task.ToList();


            List<webBinh.Models.column> columns = new List<webBinh.Models.column>();
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("http://localhost:5224/GetColumn?id_project=" + id_project).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    columns = JsonConvert.DeserializeObject<List<webBinh.Models.column>>(responseData);
                }

            }
            return View(columns.ToList());
        }

        public async Task<ActionResult> createProject([Bind(Include = "id_project,mota,title,ownerIds,createdAt")] Project project)
        {
            using (var httpClient = new HttpClient())
            {
                var url = "http://localhost:5224/createPro";
                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("id_project", project.id_project.ToString()),
            new KeyValuePair<string, string>("Mota", project.mota),
            new KeyValuePair<string, string>("Title", project.title),
            new KeyValuePair<string, string>("OwnerIds", project.ownerIds),
            new KeyValuePair<string, string>("CreatedAt", project.createdAt.ToString())
        });

                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    if (responseData.Contains("ok"))
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            return View(project);
        }

        //get prj to update
        public async Task<ActionResult> EditProject(int id)
        {
            var project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: project/editProject
        [HttpPost]
        public async Task<ActionResult> EditProject(Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                      

                        var url = $"http://localhost:5224/updatePro?id_project={project.id_project}&Mota={project.mota}&Title={project.title}";
                        var emptyContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                        var response = await httpClient.PutAsync(url, emptyContent);
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Không thể cập nhật dự án.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật: {ex.Message}");
                }
            }
            return View(project);
        }


        //del prj with id
        [HttpPost]
        public async Task<ActionResult> DeleteProject(int id_project)
        {
            try
            {
                // Gửi yêu cầu DELETE đến API
                using (var httpClient = new HttpClient())
                {
                    var apiUrl = $"http://localhost:5224/deleteProject/{id_project}";
                    var response = await httpClient.DeleteAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Lưu thông báo thành công
                        TempData["SuccessMessage"] = "Xóa project thành công.";
                    }
                    else
                    {
                        // Lưu thông báo lỗi
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"Không thể xóa project: {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
            }

            // Quay lại trang Index sau khi xử lý
            return RedirectToAction("Index");
        }


    }
}