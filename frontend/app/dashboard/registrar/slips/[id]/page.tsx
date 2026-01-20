'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function RegistrarSlipReview() {
  const params = useParams();
  const router = useRouter();
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchSlip();
  }, []);

  const fetchSlip = async () => {
    try {
      const response = await api.get(`/registrar/slips/${params.id}`);
      setData(response.data);
    } catch (error) {
      toast.error('Failed to load slip');
    } finally {
      setLoading(false);
    }
  };

  const handleFinalize = async () => {
    try {
      await api.post(`/registrar/slips/${params.id}/finalize`);
      toast.success('Registration finalized successfully');
      router.push('/dashboard/registrar');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Finalization failed');
    }
  };

  if (loading) {
    return (
      <Layout role="Registrar">
        <div>Loading...</div>
      </Layout>
    );
  }

  if (!data?.slip) {
    return (
      <Layout role="Registrar">
        <div>Slip not found</div>
      </Layout>
    );
  }

  const slip = data.slip;

  return (
    <Layout role="Registrar">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Final Registration Review</h1>
        
        <div className="mb-6">
          <h3 className="font-semibold mb-2">Student Information</h3>
          <p><strong>Name:</strong> {slip.studentName}</p>
          <p><strong>Student ID:</strong> {slip.studentID}</p>
          <p><strong>Department:</strong> {slip.department}</p>
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
          <h3 className="font-semibold mb-2">Approval Status</h3>
          <div className="space-y-2">
            <div className={slip.isAdvisorApproved ? 'text-green-600' : 'text-gray-400'}>
              {slip.isAdvisorApproved ? '✓' : '○'} Advisor Approved
            </div>
            <div className={slip.isCostSharingVerified ? 'text-green-600' : 'text-gray-400'}>
              {slip.isCostSharingVerified ? '✓' : '○'} Cost Sharing Verified
            </div>
          </div>
        </div>

        {data.costSharingForm && (
          <div className="mb-6">
            <h3 className="font-semibold mb-2">Cost Sharing Form</h3>
            <p><strong>Status:</strong> {data.costSharingForm.status}</p>
            {data.costSharingForm.paymentInfo && (
              <p><strong>Payment Info:</strong> {data.costSharingForm.paymentInfo}</p>
            )}
          </div>
        )}

        {!slip.isRegistrarFinalized && (
          <button
            onClick={handleFinalize}
            className="bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700"
          >
            Finalize Registration
          </button>
        )}

        {slip.isRegistrarFinalized && (
          <div>
            <p className="text-green-600 font-semibold mb-4">✓ Registration Finalized</p>
            {slip.serialNumber && <p><strong>Serial Number:</strong> {slip.serialNumber}</p>}
            <a
              href={`/api/registrar/slips/${slip.id}/pdf`}
              className="inline-block mt-4 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
              target="_blank"
            >
              Download PDF
            </a>
          </div>
        )}
      </div>
    </Layout>
  );
}
