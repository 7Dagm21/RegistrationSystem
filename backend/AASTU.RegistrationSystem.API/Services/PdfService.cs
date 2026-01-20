using iTextSharp.text;
using iTextSharp.text.pdf;
using AASTU.RegistrationSystem.API.DTOs;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Services
{
    public class PdfService : IPdfService
    {
        public async Task<byte[]> GenerateApprovedSlipPdfAsync(RegistrationSlipDto slip)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Title
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("AASTU Registration Slip", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                // Student Information
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                document.Add(new Paragraph("Student Information:", headerFont));
                document.Add(new Paragraph($"Name: {slip.StudentName}", normalFont));
                document.Add(new Paragraph($"Student ID: {slip.StudentID}", normalFont));
                document.Add(new Paragraph($"Department: {slip.Department}", normalFont));
                document.Add(new Paragraph($"Academic Year: {slip.AcademicYear}", normalFont));
                document.Add(new Paragraph($"Semester: {slip.Semester}", normalFont));
                document.Add(new Paragraph(" "));

                // Courses
                document.Add(new Paragraph("Registered Courses:", headerFont));
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 4, 1 });

                table.AddCell(new PdfPCell(new Phrase("Course Code", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Course Name", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Credit Hours", headerFont)));

                foreach (var course in slip.Courses)
                {
                    table.AddCell(new PdfPCell(new Phrase(course.CourseCode, normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(course.CourseName, normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(course.CreditHours.ToString(), normalFont)));
                }

                document.Add(table);
                document.Add(new Paragraph($"Total Credit Hours: {slip.TotalCreditHours}", headerFont));
                document.Add(new Paragraph(" "));

                // Approvals
                document.Add(new Paragraph("Approvals:", headerFont));
                if (slip.IsAdvisorApproved)
                    document.Add(new Paragraph("✓ Advisor Approved", normalFont));
                if (slip.IsCostSharingVerified)
                    document.Add(new Paragraph("✓ Cost Sharing Verified", normalFont));
                if (slip.IsRegistrarFinalized)
                    document.Add(new Paragraph("✓ Registrar Finalized", normalFont));

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph($"Serial Number: {slip.SerialNumber}", headerFont));
                document.Add(new Paragraph($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", normalFont));

                document.Close();

                return await Task.FromResult(ms.ToArray());
            }
        }

        public async Task<byte[]> GenerateCostSharingFormPdfAsync(CostSharingForm form, Student student, RegistrationSlip slip)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                // Header
                Paragraph title = new Paragraph("ADDIS ABABA SCIENCE AND TECHNOLOGY UNIVERSITY", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 5;
                document.Add(title);

                Paragraph subtitle = new Paragraph("COST-SHARING BENEFICIARIES AGREEMENT FORM", headerFont);
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 20;
                document.Add(subtitle);

                // Document Info
                Paragraph docInfo = new Paragraph($"Document No: VPAA/REG/OF/013 | Page 1 of 3", smallFont);
                docInfo.Alignment = Element.ALIGN_RIGHT;
                docInfo.SpacingAfter = 15;
                document.Add(docInfo);

                // Add student photo if available (top right corner)
                if (!string.IsNullOrEmpty(form.PhotoDataUrl) && form.PhotoDataUrl.StartsWith("data:image"))
                {
                    try
                    {
                        var base64Data = form.PhotoDataUrl.Split(',')[1];
                        var imageBytes = Convert.FromBase64String(base64Data);
                        var photo = iTextSharp.text.Image.GetInstance(imageBytes);
                        photo.ScaleToFit(100, 120);
                        photo.SetAbsolutePosition(document.PageSize.Width - 150, document.PageSize.Height - 150);
                        document.Add(photo);
                    }
                    catch
                    {
                        // If image fails to load, continue without it
                    }
                }

                // Section 1: Full Name
                document.Add(new Paragraph("1. Full Name (including grandfather's name):", headerFont));
                document.Add(new Paragraph($"{form.FullName ?? student.FullName}", normalFont));
                document.Add(new Paragraph($"Identity No.: {form.IdentityNo ?? student.StudentID}", normalFont));
                document.Add(new Paragraph(" "));

                // Section 2: Sex and Nationality
                string sexDisplay = form.Sex == "Male" ? "☑ Male  ☐ Female" : (form.Sex == "Female" ? "☐ Male  ☑ Female" : "☐ Male  ☐ Female");
                document.Add(new Paragraph($"2. Sex: {sexDisplay}", normalFont));
                document.Add(new Paragraph($"Nationality: {form.Nationality ?? "Ethiopian"}", normalFont));
                document.Add(new Paragraph(" "));

                // Section 3: Date of Birth
                document.Add(new Paragraph("3. Date of Birth:", headerFont));
                if (form.DateOfBirth.HasValue)
                {
                    document.Add(new Paragraph($"Date: {form.DateOfBirth.Value.Day:00}  Month: {form.DateOfBirth.Value.Month:00}  Year: {form.DateOfBirth.Value.Year}", normalFont));
                }
                else
                {
                    document.Add(new Paragraph($"Date: __  Month: __  Year: ____", normalFont));
                }
                document.Add(new Paragraph(" "));

                // Section 4: Place of Birth
                if (!string.IsNullOrEmpty(form.PlaceOfBirth))
                {
                    document.Add(new Paragraph("4. Place of Birth:", headerFont));
                    document.Add(new Paragraph(form.PlaceOfBirth, normalFont));
                    document.Add(new Paragraph(" "));
                }

                // Section 4: Mother's/Adopter's Information
                document.Add(new Paragraph("4. Mother's/Adopter's - Full Name:", headerFont));
                document.Add(new Paragraph(form.MothersFullName ?? "[Not provided]", normalFont));
                if (!string.IsNullOrEmpty(form.MothersAddress))
                {
                    document.Add(new Paragraph($"Address: {form.MothersAddress}", normalFont));
                }
                document.Add(new Paragraph(" "));

                // Section 5: School Information
                document.Add(new Paragraph("5. School Name (Where you completed your preparatory program):", headerFont));
                document.Add(new Paragraph(form.SchoolName ?? "[Not provided]", normalFont));
                if (form.DateCompleted.HasValue)
                {
                    document.Add(new Paragraph($"Date completed: {form.DateCompleted.Value:dd MMM yyyy}", normalFont));
                }
                document.Add(new Paragraph(" "));

                // Section 6: University Information
                document.Add(new Paragraph("6. University/College/Institute:", headerFont));
                document.Add(new Paragraph("Addis Ababa Science and Technology University", normalFont));
                document.Add(new Paragraph($"Faculty/College/Institute/School: {form.FacultyOrCollege ?? student.Department}", normalFont));
                document.Add(new Paragraph($"Year of entrance: {form.EntranceYearEC ?? student.EnrollmentYear.ToString()} E.C.", normalFont));
                document.Add(new Paragraph($"Department: {form.Department ?? student.Department}", normalFont));
                document.Add(new Paragraph($"Year: {form.AcademicYearText ?? slip.AcademicYear.ToString()}", normalFont));
                document.Add(new Paragraph(" "));

                // Page break for Page 2
                document.NewPage();

                Paragraph page2Title = new Paragraph("ADDIS ABABA SCIENCE AND TECHNOLOGY UNIVERSITY", titleFont);
                page2Title.Alignment = Element.ALIGN_CENTER;
                page2Title.SpacingAfter = 5;
                document.Add(page2Title);

                Paragraph page2Subtitle = new Paragraph("COST-SHARING BENEFICIARIES AGREEMENT FORM", headerFont);
                page2Subtitle.Alignment = Element.ALIGN_CENTER;
                page2Subtitle.SpacingAfter = 5;
                document.Add(page2Subtitle);

                Paragraph page2Info = new Paragraph("Page 2 of 3", smallFont);
                page2Info.Alignment = Element.ALIGN_RIGHT;
                page2Info.SpacingAfter = 15;
                document.Add(page2Info);

                // Section 9: Services
                document.Add(new Paragraph("9. What services would you demand? (Please mark \"X\")", headerFont));
                
                // Parse service selection JSON
                string inKind = "";
                string inCash = "";
                if (!string.IsNullOrEmpty(form.ServiceSelection))
                {
                    try
                    {
                        var serviceData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(form.ServiceSelection);
                        inKind = serviceData?.GetValueOrDefault("inKind", "") ?? "";
                        inCash = serviceData?.GetValueOrDefault("inCash", "") ?? "";
                    }
                    catch { }
                }

                document.Add(new Paragraph("A) In kind:", normalFont));
                document.Add(new Paragraph($"   {(inKind == "Food only" ? "☑" : "☐")} 1. Food only", normalFont));
                document.Add(new Paragraph($"   {(inKind == "Boarding only" ? "☑" : "☐")} 2. Boarding only", normalFont));
                document.Add(new Paragraph($"   {(inKind == "Food and Boarding" ? "☑" : "☐")} 3. Food and Boarding", normalFont));
                document.Add(new Paragraph("B) In cash:", normalFont));
                document.Add(new Paragraph($"   {(inCash == "Food only" ? "☑" : "☐")} 1. Food only", normalFont));
                document.Add(new Paragraph($"   {(inCash == "Boarding only" ? "☑" : "☐")} 2. Boarding only", normalFont));
                document.Add(new Paragraph($"   {(inCash == "Food and Boarding" ? "☑" : "☐")} 3. Food and Boarding", normalFont));
                document.Add(new Paragraph(" "));

                // Section 10: Estimated Cost
                document.Add(new Paragraph("10. Estimated cost to be borne by the beneficiary in the current academic year:", headerFont));
                
                PdfPTable costTable = new PdfPTable(2);
                costTable.WidthPercentage = 100;
                costTable.SetWidths(new float[] { 3, 2 });

                costTable.AddCell(new PdfPCell(new Phrase("15% tuition fee (Birr):", normalFont)));
                costTable.AddCell(new PdfPCell(new Phrase($"{form.TuitionFee15Percent:N2}", normalFont)));

                costTable.AddCell(new PdfPCell(new Phrase("Food expense (Birr):", normalFont)));
                costTable.AddCell(new PdfPCell(new Phrase($"{form.FoodExpense:N2}", normalFont)));

                costTable.AddCell(new PdfPCell(new Phrase("Boarding expense (Birr):", normalFont)));
                costTable.AddCell(new PdfPCell(new Phrase($"{form.BoardingExpense:N2}", normalFont)));

                costTable.AddCell(new PdfPCell(new Phrase("Total (Birr):", headerFont)));
                costTable.AddCell(new PdfPCell(new Phrase($"{form.TotalCost:N2}", headerFont)));

                document.Add(costTable);
                document.Add(new Paragraph(" "));

                // Section 11: Advance Payment
                document.Add(new Paragraph("11. Date of advance payment, if any (Date Month Year) Discount Receipt No.:", headerFont));
                if (form.AdvancePaymentDate.HasValue)
                {
                    document.Add(new Paragraph($"Date: {form.AdvancePaymentDate.Value:dd}  Month: {form.AdvancePaymentDate.Value:MMM}  Year: {form.AdvancePaymentDate.Value:yyyy}", normalFont));
                }
                if (!string.IsNullOrEmpty(form.Discount))
                {
                    document.Add(new Paragraph($"Discount: {form.Discount}", normalFont));
                }
                if (!string.IsNullOrEmpty(form.ReceiptNo))
                {
                    document.Add(new Paragraph($"Receipt No.: {form.ReceiptNo}", normalFont));
                }
                document.Add(new Paragraph(" "));

                // Section 12: Agreement
                document.Add(new Paragraph("12. In accordance with Higher Education proclamation No.351/1995 and Higher education Cost Sharing Regulation 154/2008:", headerFont));
                document.Add(new Paragraph("I agree to pay the cost-sharing amount:", normalFont));
                document.Add(new Paragraph("A) To be paid from my income; or", normalFont));
                document.Add(new Paragraph("B) To provide service not more than the training period in my profession.", normalFont));
                document.Add(new Paragraph(" "));

                // Page break for Page 3
                document.NewPage();

                Paragraph page3Title = new Paragraph("ADDIS ABABA SCIENCE AND TECHNOLOGY UNIVERSITY", titleFont);
                page3Title.Alignment = Element.ALIGN_CENTER;
                page3Title.SpacingAfter = 5;
                document.Add(page3Title);

                Paragraph page3Subtitle = new Paragraph("COST-SHARING BENEFICIARIES AGREEMENT FORM", headerFont);
                page3Subtitle.Alignment = Element.ALIGN_CENTER;
                page3Subtitle.SpacingAfter = 5;
                document.Add(page3Subtitle);

                Paragraph page3Info = new Paragraph("Page 3 of 3", smallFont);
                page3Info.Alignment = Element.ALIGN_RIGHT;
                page3Info.SpacingAfter = 15;
                document.Add(page3Info);

                // Certification
                document.Add(new Paragraph("I also certify that the above information is true.", normalFont));
                document.Add(new Paragraph(" "));

                // Beneficiary Signature
                document.Add(new Paragraph("Beneficiary's Signature:", headerFont));
                document.Add(new Paragraph(form.BeneficiarySignatureName ?? "_________________________", normalFont));
                if (form.BeneficiarySignedAt.HasValue)
                {
                    document.Add(new Paragraph($"Date: {form.BeneficiarySignedAt.Value:dd}  Month: {form.BeneficiarySignedAt.Value:MM}  Year: {form.BeneficiarySignedAt.Value:yyyy}", normalFont));
                }
                else
                {
                    document.Add(new Paragraph($"Date: {DateTime.Now:dd}  Month: {DateTime.Now:MM}  Year: {DateTime.Now:yyyy}", normalFont));
                }
                document.Add(new Paragraph(" "));

                // Institute Certification
                document.Add(new Paragraph("13. I, the undersigned, certify that the above-mentioned beneficiary had signed this contract in our office and the above stated amount is correct.", normalFont));
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("Head/Representative of the institute:", headerFont));
                document.Add(new Paragraph("Name: _________________________", normalFont));
                document.Add(new Paragraph("Signature: _________________________", normalFont));
                document.Add(new Paragraph($"Date: {DateTime.Now:dd}  Month: {DateTime.Now:MM}  Year: {DateTime.Now:yyyy}", normalFont));
                document.Add(new Paragraph(" "));

                // Notes
                document.Add(new Paragraph("Notes:", headerFont));
                document.Add(new Paragraph("A. If the beneficiary leaves the institution for any reason, they are obligated to pay the expenses incurred until their departure.", smallFont));
                document.Add(new Paragraph("B. This form is made in 3 copies; one will be given to the student, one to the Registrar, and one to the Cost-Sharing Office.", smallFont));
                document.Add(new Paragraph(" "));

                // Incentives
                document.Add(new Paragraph("Incentives:", headerFont));
                document.Add(new Paragraph("1. Ten percent (10%) for beneficiaries who pay the full cost-sharing amount in advance.", smallFont));
                document.Add(new Paragraph("2. Five percent (5%) for those who pay in advance at the beginning of each year.", smallFont));
                document.Add(new Paragraph("3. Three percent (3%) for those who pay within one year during the grace period after graduation.", smallFont));

                document.Close();

                return await Task.FromResult(ms.ToArray());
            }
        }

        public async Task<byte[]> GenerateGradeReportPdfAsync(GradeReport gradeReport)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                // Header
                Paragraph title = new Paragraph("ADDIS ABABA SCIENCE AND TECHNOLOGY UNIVERSITY", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 5;
                document.Add(title);

                Paragraph college = new Paragraph("College of Engineering", headerFont);
                college.Alignment = Element.ALIGN_CENTER;
                college.SpacingAfter = 10;
                document.Add(college);

                Paragraph reportTitle = new Paragraph("Grade Report", headerFont);
                reportTitle.Alignment = Element.ALIGN_CENTER;
                reportTitle.SpacingAfter = 15;
                document.Add(reportTitle);

                // Student Information Table
                PdfPTable infoTable = new PdfPTable(4);
                infoTable.WidthPercentage = 100;
                infoTable.SetWidths(new float[] { 2, 3, 2, 3 });

                infoTable.AddCell(new PdfPCell(new Phrase("Admission Classification:", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase(gradeReport.AdmissionClassification ?? "Regular", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase("Program:", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase(gradeReport.Program ?? "Degree", normalFont)) { Border = PdfPCell.NO_BORDER });

                infoTable.AddCell(new PdfPCell(new Phrase("Year:", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase(gradeReport.Year.ToString(), normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase("Student Name:", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase(gradeReport.StudentName ?? "", normalFont)) { Border = PdfPCell.NO_BORDER });

                infoTable.AddCell(new PdfPCell(new Phrase("Major:", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase(gradeReport.Major ?? "", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase("Academic Year:", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase(gradeReport.AcademicYear, normalFont)) { Border = PdfPCell.NO_BORDER });

                infoTable.AddCell(new PdfPCell(new Phrase("Semester:", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase(gradeReport.Semester, normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase("Student ID:", normalFont)) { Border = PdfPCell.NO_BORDER });
                infoTable.AddCell(new PdfPCell(new Phrase(gradeReport.StudentID, normalFont)) { Border = PdfPCell.NO_BORDER });

                document.Add(infoTable);
                document.Add(new Paragraph(" "));

                // Courses Table
                var courses = System.Text.Json.JsonSerializer.Deserialize<List<CourseGradeDto>>(gradeReport.GradesJson) ?? new List<CourseGradeDto>();

                PdfPTable courseTable = new PdfPTable(7);
                courseTable.WidthPercentage = 100;
                courseTable.SetWidths(new float[] { 1.5f, 3, 1, 1, 1, 1, 1 });

                courseTable.AddCell(new PdfPCell(new Phrase("Course Code", headerFont)));
                courseTable.AddCell(new PdfPCell(new Phrase("Course Title", headerFont)));
                courseTable.AddCell(new PdfPCell(new Phrase("Credit", headerFont)));
                courseTable.AddCell(new PdfPCell(new Phrase("Number Grade", headerFont)));
                courseTable.AddCell(new PdfPCell(new Phrase("Letter Grade", headerFont)));
                courseTable.AddCell(new PdfPCell(new Phrase("Grade Point", headerFont)));
                courseTable.AddCell(new PdfPCell(new Phrase("Total", headerFont)));

                foreach (var course in courses)
                {
                    courseTable.AddCell(new PdfPCell(new Phrase(course.CourseCode, normalFont)));
                    courseTable.AddCell(new PdfPCell(new Phrase(course.CourseTitle, normalFont)));
                    courseTable.AddCell(new PdfPCell(new Phrase(course.Credit.ToString("F2"), normalFont)));
                    courseTable.AddCell(new PdfPCell(new Phrase(course.NumberGrade.ToString("F2"), normalFont)));
                    courseTable.AddCell(new PdfPCell(new Phrase(course.LetterGrade, normalFont)));
                    courseTable.AddCell(new PdfPCell(new Phrase(course.GradePoint.ToString("F2"), normalFont)));
                    courseTable.AddCell(new PdfPCell(new Phrase(course.GradePoint.ToString("F2"), normalFont)));
                }

                document.Add(courseTable);
                document.Add(new Paragraph(" "));

                // Summary Table
                PdfPTable summaryTable = new PdfPTable(4);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 2, 1, 1, 1 });

                summaryTable.AddCell(new PdfPCell(new Phrase("", normalFont)) { Border = PdfPCell.NO_BORDER });
                summaryTable.AddCell(new PdfPCell(new Phrase("Credit", headerFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase("GP", headerFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase("ANG", headerFont)));

                summaryTable.AddCell(new PdfPCell(new Phrase("Previous Total:", normalFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.PreviousCredit?.ToString("F2") ?? "0.00", normalFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.PreviousGP?.ToString("F2") ?? "0.00", normalFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.PreviousANG?.ToString("F2") ?? "0.00", normalFont)));

                summaryTable.AddCell(new PdfPCell(new Phrase("Semester Total:", normalFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.SemesterCredit?.ToString("F2") ?? "0.00", normalFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.SemesterGP?.ToString("F2") ?? "0.00", normalFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.SemesterANG?.ToString("F2") ?? "0.00", normalFont)));

                summaryTable.AddCell(new PdfPCell(new Phrase("Cumulative:", headerFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.CumulativeCredit?.ToString("F2") ?? "0.00", headerFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.CumulativeGP?.ToString("F2") ?? "0.00", headerFont)));
                summaryTable.AddCell(new PdfPCell(new Phrase(gradeReport.CumulativeANG?.ToString("F2") ?? "0.00", headerFont)));

                document.Add(summaryTable);
                document.Add(new Paragraph(" "));

                // Remark
                document.Add(new Paragraph($"Remark: {gradeReport.Remark ?? "X Promoted"}", normalFont));
                document.Add(new Paragraph(" "));

                // Registrar Information
                document.Add(new Paragraph($"Registrar Recorder Name: {gradeReport.RegistrarRecorderName ?? ""}", normalFont));
                document.Add(new Paragraph($"Signature: _________________________", normalFont));
                document.Add(new Paragraph($"Date: {gradeReport.RegistrarSignedDate?.ToString("dd MMM yyyy") ?? DateTime.Now.ToString("dd MMM yyyy")}", normalFont));
                document.Add(new Paragraph(" "));

                // Generated By
                document.Add(new Paragraph($"Report is generated by {gradeReport.GeneratedBy ?? ""} on: {gradeReport.GeneratedAt?.ToString("M/d/yyyy h:mm tt") ?? DateTime.Now.ToString("M/d/yyyy h:mm tt")}", smallFont));

                document.Close();

                return await Task.FromResult(ms.ToArray());
            }
        }
    }
}
