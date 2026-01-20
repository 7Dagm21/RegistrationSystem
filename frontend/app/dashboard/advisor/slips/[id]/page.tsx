'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function AdvisorSlipReview() {
  const params = useParams();
  const router = useRouter();
  const [slip, setSlip] = useState<any>(null);
  const [comment, setComment] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchSlip();
  }, []);

  const fetchSlip = async () => {
    try {
      const response = await api.get(`/advisor/slips/${params.id}`);
      setSlip(response.data);
    } catch (error) {
      toast.error('Failed to load slip');
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async () => {
    try {
      await api.post(`/advisor/slips/${params.id}/approve`, {
        isApproved: true,
        comment: comment,
      });
      toast.success('Slip approved successfully');
      router.push('/dashboard/advisor/pending');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Approval failed');
    }
  };

  const handleReject = async () => {
    if (!comment.trim()) {
      toast.error('Please provide a comment for rejection');
      return;
    }
    try {
      await api.post(`/advisor/slips/${params.id}/reject`, {
        isApproved: false,
        comment: comment,
      });
      toast.success('Slip rejected');
      router.push('/dashboard/advisor/pending');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Rejection failed');
    }
  };

  if (loading) {
    return (
      <Layout role="Advisor">
        <div>Loading...</div>
      </Layout>
    );
  }

  if (!slip) {
    return (
      <Layout role="Advisor">
        <div>Slip not found</div>
      </Layout>
    );
  }

  return (
    <Layout role="Advisor">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Review Registration Slip</h1>
        
        <div className="mb-6">
          <h3 className="font-semibold mb-2">Student Information</h3>
          <p><strong>Name:</strong> {slip.studentName}</p>
          <p><strong>Student ID:</strong> {slip.studentID}</p>
          <p><strong>Department:</strong> {slip.department}</p>
          <p><strong>Academic Year:</strong> {slip.academicYear}</p>
          <p><strong>Semester:</strong> {slip.semester}</p>
        </div>

        <div className="mb-6">
          <h3 className="font-semibold mb-2">Registered Courses</h3>
          <table className="w-full border-collapse border">
            <thead>
              <tr className="bg-gray-100">
                <th className="border p-2">Course Code</th>
                <th className="border p-2">Course Name</th>
                <th className="border p-2">Credit Hours</th>
              </tr>
            </thead>
            <tbody>
              {slip.courses.map((course: any, index: number) => (
                <tr key={index}>
                  <td className="border p-2">{course.courseCode}</td>
                  <td className="border p-2">{course.courseName}</td>
                  <td className="border p-2">{course.creditHours}</td>
                </tr>
              ))}
            </tbody>
          </table>
          <p className="mt-2"><strong>Total Credit Hours:</strong> {slip.totalCreditHours}</p>
        </div>

        <div className="mb-6">
          <label className="block text-sm font-medium mb-2">Comment</label>
          <textarea
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            className="w-full px-3 py-2 border rounded-lg"
            rows={4}
            placeholder="Enter your comment (required for rejection)"
          />
        </div>

        <div className="flex gap-4">
          <button
            onClick={handleApprove}
            className="bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700"
          >
            Approve
          </button>
          <button
            onClick={handleReject}
            className="bg-red-600 text-white px-6 py-2 rounded hover:bg-red-700"
          >
            Reject
          </button>
        </div>
      </div>
    </Layout>
  );
}
