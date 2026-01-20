"use client";

import { useEffect, useState } from "react";
import Layout from "@/components/Layout";
import api from "@/lib/api";
import { toast } from "react-toastify";
import Link from "next/link";

export default function PendingGradeReports() {
  const [gradeReports, setGradeReports] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchPendingGradeReports();
  }, []);

  const fetchPendingGradeReports = async () => {
    setLoading(true);
    try {
      const response = await api.get("/GradeReport/pending-approvals");
      setGradeReports(response.data);
    } catch (error: any) {
      toast.error(
        error.response?.data?.message || "Failed to load pending grade reports",
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout role="DepartmentHead">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Pending Grade Reports</h1>
        {loading ? (
          <div>Loading...</div>
        ) : gradeReports.length === 0 ? (
          <div className="text-gray-600">No pending grade reports.</div>
        ) : (
          <table className="w-full border-collapse border">
            <thead>
              <tr className="bg-gray-100">
                <th className="border px-4 py-2">Student ID</th>
                <th className="border px-4 py-2">Student Name</th>
                <th className="border px-4 py-2">Major</th>
                <th className="border px-4 py-2">Semester</th>
                <th className="border px-4 py-2">Academic Year</th>
                <th className="border px-4 py-2">Year</th>
                <th className="border px-4 py-2">Action</th>
              </tr>
            </thead>
            <tbody>
              {gradeReports.map((report) => (
                <tr key={report.id}>
                  <td className="border px-4 py-2">{report.studentID}</td>
                  <td className="border px-4 py-2">{report.studentName}</td>
                  <td className="border px-4 py-2">{report.major}</td>
                  <td className="border px-4 py-2">{report.semester}</td>
                  <td className="border px-4 py-2">{report.academicYear}</td>
                  <td className="border px-4 py-2">{report.year}</td>
                  <td className="border px-4 py-2">
                    <Link
                      href={`/dashboard/department-head/grade-report/${report.id}`}
                      className="text-blue-600 hover:underline"
                    >
                      View & Approve
                    </Link>
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
