"use client";

import { useEffect, useState } from "react";
import Layout from "@/components/Layout";
import api from "@/lib/api";
import { useRouter, useParams } from "next/navigation";
import { toast } from "react-toastify";

export default function GradeReportDetail() {
  const router = useRouter();
  const params = useParams();
  const { id } = params;
  const [gradeReport, setGradeReport] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [approving, setApproving] = useState(false);
  const [rejecting, setRejecting] = useState(false);
  const [comment, setComment] = useState("");

  useEffect(() => {
    if (id) fetchGradeReport();
    // eslint-disable-next-line
  }, [id]);

  const fetchGradeReport = async () => {
    setLoading(true);
    try {
      const response = await api.get(`/GradeReport/${id}`);
      setGradeReport(response.data);
    } catch (error: any) {
      toast.error(
        error.response?.data?.message || "Failed to load grade report",
      );
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async () => {
    setApproving(true);
    try {
      await api.post(`/GradeReport/${id}/approve`, { isApproved: true });
      toast.success("Grade report approved");
      router.push("/dashboard/department-head/grade-report");
    } catch (error: any) {
      toast.error(error.response?.data?.message || "Failed to approve");
    } finally {
      setApproving(false);
    }
  };

  const handleReject = async () => {
    setRejecting(true);
    try {
      await api.post(`/GradeReport/${id}/approve`, {
        isApproved: false,
        comment,
      });
      toast.success("Grade report rejected");
      router.push("/dashboard/department-head/grade-report");
    } catch (error: any) {
      toast.error(error.response?.data?.message || "Failed to reject");
    } finally {
      setRejecting(false);
    }
  };

  if (loading) {
    return (
      <Layout role="DepartmentHead">
        <div>Loading...</div>
      </Layout>
    );
  }

  if (!gradeReport) {
    return (
      <Layout role="DepartmentHead">
        <div className="text-red-600">Grade report not found.</div>
      </Layout>
    );
  }

  return (
    <Layout role="DepartmentHead">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Grade Report Detail</h1>
        <div className="mb-4">
          <strong>Student ID:</strong> {gradeReport.studentID}
          <br />
          <strong>Student Name:</strong> {gradeReport.studentName}
          <br />
          <strong>Major:</strong> {gradeReport.major}
          <br />
          <strong>Semester:</strong> {gradeReport.semester}
          <br />
          <strong>Academic Year:</strong> {gradeReport.academicYear}
          <br />
          <strong>Year:</strong> {gradeReport.year}
          <br />
        </div>
        <h2 className="font-semibold mb-2">Courses</h2>
        <table className="w-full border-collapse border mb-4">
          <thead>
            <tr className="bg-gray-100">
              <th className="border px-4 py-2">Course Code</th>
              <th className="border px-4 py-2">Course Title</th>
              <th className="border px-4 py-2">Credit</th>
              <th className="border px-4 py-2">Number Grade</th>
              <th className="border px-4 py-2">Letter Grade</th>
              <th className="border px-4 py-2">Grade Point</th>
            </tr>
          </thead>
          <tbody>
            {gradeReport.courses.map((course: any, idx: number) => (
              <tr key={idx}>
                <td className="border px-4 py-2">{course.courseCode}</td>
                <td className="border px-4 py-2">{course.courseTitle}</td>
                <td className="border px-4 py-2">{course.credit}</td>
                <td className="border px-4 py-2">{course.numberGrade}</td>
                <td className="border px-4 py-2">{course.letterGrade}</td>
                <td className="border px-4 py-2">{course.gradePoint}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="mb-4">
          <strong>GPA:</strong> {gradeReport.gpa} <br />
          <strong>CGPA:</strong> {gradeReport.cgpa}
        </div>
        <div className="flex gap-4">
          <button
            onClick={handleApprove}
            disabled={approving}
            className="bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700 disabled:opacity-50"
          >
            {approving ? "Approving..." : "Approve"}
          </button>
          <input
            type="text"
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Rejection reason (optional)"
            className="border px-3 py-2 rounded-lg"
          />
          <button
            onClick={handleReject}
            disabled={rejecting}
            className="bg-red-600 text-white px-6 py-2 rounded hover:bg-red-700 disabled:opacity-50"
          >
            {rejecting ? "Rejecting..." : "Reject"}
          </button>
        </div>
      </div>
    </Layout>
  );
}
