using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.DataLayer;

namespace CollegeConnected.Controllers
{
    public class BaseController : Controller
    {
        private readonly UnitOfWork db = new UnitOfWork();

        public SelectList GenerateGradYearList()
        {
            return new SelectList(Enumerable.Range(1940, 100).Select(x =>
                new SelectListItem
                {
                    Text = x.ToString(),
                    Value = x.ToString()
                }), "Value", "Text");
            ;
        }

        public bool IsAuthenticated(HttpCookie cookie)
        {
            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);

                if (ticket != null)
                    return true;
            }
            return false;
        }

        public void ExportToCsv(Guid? id, string exportType)
        {
            var sw = new StringWriter();

            sw.WriteLine("\"Student Number\",\"First Name\",\"Middle Name\",\"Last Name\",\"Address1\"," +
                         "\"Address2\",\"Zip Code\",\"City\",\"State\",\"Phone Number\",\"Email\",\"Graduation Year" +
                         "\"Birthday\",\"First Grad Year\",\"Second Grad Year\",\"Third Grad Year\",\"Constiuent Type\",\"Allow Communication\"");
            Response.ClearContent();
            Response.AddHeader("content-disposition",
                "attachment;filename=ExportedConstituents_" + DateTime.Now + ".csv");
            Response.ContentType = "text/csv";

            if (string.Equals(exportType, "event"))
            {
                var attendees = db.EventAttendanceRepository.Get(ev => ev.EventId == id).ToList();
                foreach (var attendee in attendees)
                {
                    var student = db.StudentRepository.GetById(attendee.StudentId);
                    sw.WriteLine(
                        $"\"{student.StudentNumber}\",\"{student.FirstName}\",\"{student.MiddleName}\",\"{student.LastName}\",\"{student.Address1}\"," +
                        $"\"{student.Address2}\",\"{student.ZipCode}\",\"{student.City}\",\"{student.State}\",\"{student.PhoneNumber}\",\"{student.Email}\"," +
                        $"\"{student.FirstGraduationYear}\",\"{student.BirthDate}\",\"{student.FirstGraduationYear}\",\"{student.SecondGraduationYear}\",\"{student.ThirdGraduationYear}\"" +
                        $",\"{student.ConstituentType}\",\"{student.AllowCommunication}\"");
                }
                Response.Write(sw.ToString());
                Response.End();
            }
            else
            {
                var students = db.StudentRepository.Get();
                foreach (var student in students)
                    sw.WriteLine(
                        $"\"{student.StudentNumber}\",\"{student.FirstName}\",\"{student.MiddleName}\",\"{student.LastName}\",\"{student.Address1}\"," +
                        $"\"{student.Address2}\",\"{student.ZipCode}\",\"{student.City}\",\"{student.State}\",\"{student.PhoneNumber}\",\"{student.Email}\"," +
                        $"\"{student.FirstGraduationYear}\",\"{student.BirthDate}\",\"{student.FirstGraduationYear}\",\"{student.SecondGraduationYear}\",\"{student.ThirdGraduationYear}\"" +
                        $",\"{student.ConstituentType}\",\"{student.AllowCommunication}\"");
                Response.Write(sw.ToString());
                Response.End();
            }
        }
    }
}