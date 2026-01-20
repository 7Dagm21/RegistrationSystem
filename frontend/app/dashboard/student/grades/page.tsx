"use client";

import { useEffect, useState } from "react";
import Layout from "@/components/Layout";
import api from "@/lib/api";
import { toast } from "react-toastify";

export default function GradeReports() {
  const [reports, setReports] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchReports();
  }, []);

  const fetchReports = async () => {
    try {
      const response = await api.get("/student/grade-reports");
      setReports(response.data);
    } catch (error) {
      toast.error("Failed to load grade reports");
    } finally {
      setLoading(false);
    }
  };

  const handleDownload = async (id: number) => {
    try {
      const response = await api.get(`/student/grade-reports/${id}/pdf`, {
        responseType: "blob",
      });
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `GradeReport_${id}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (error) {
      toast.error("Failed to download PDF");
    }
  };

  if (loading) {
    return (
      <Layout role="Student">
        <div>Loading...</div>
      </Layout>
    );
  }

  return (
    <Layout role="Student">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">My Grade Reports</h1>
        {reports.length === 0 ? (
          <p className="text-gray-600">No grade reports available.</p>
        ) : (
          <table className="w-full border-collapse border">
            <thead>
              <tr className="bg-gray-100">
                <th className="border px-4 py-2">Report ID</th>
                <th className="border px-4 py-2">Semester</th>
                <th className="border px-4 py-2">Academic Year</th>
                <th className="border px-4 py-2">GPA</th>
                <th className="border px-4 py-2">CGPA</th>
                <th className="border px-4 py-2">Status</th>
                <th className="border px-4 py-2">Action</th>
              </tr>
            </thead>
            <tbody>
              {reports.map((report) => (
                <tr key={report.id}>
                  <td className="border px-4 py-2">{report.id}</td>
                  <td className="border px-4 py-2">{report.semester}</td>
                  <td className="border px-4 py-2">{report.academicYear}</td>
                  <td className="border px-4 py-2">{report.gpa}</td>
                  <td className="border px-4 py-2">{report.cgpa}</td>
                  <td className="border px-4 py-2">{report.status}</td>
                  <td className="border px-4 py-2">
                    <button
                      onClick={() => handleDownload(report.id)}
                      className="text-blue-600 hover:underline"
                    >
                      Download PDF
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </Layout>
  );
}
