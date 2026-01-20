'use client';

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';
import dynamic from 'next/dynamic';

const QRCodeSVG = dynamic(() => import('qrcode.react').then(mod => mod.QRCodeSVG), { ssr: false });

export default function SlipDetails() {
  const params = useParams();
  const [slip, setSlip] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchSlip();
  }, []);

  const fetchSlip = async () => {
    try {
      const response = await api.get(`/student/slips/${params.id}`);
      setSlip(response.data);
    } catch (error) {
      toast.error('Failed to load slip');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout role="Student">
        <div>Loading...</div>
      </Layout>
    );
  }

  if (!slip) {
    return (
      <Layout role="Student">
        <div>Slip not found</div>
      </Layout>
    );
  }

  return (
    <Layout role="Student">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Registration Slip</h1>
        
        <div className="grid md:grid-cols-2 gap-6 mb-6">
          <div>
            <h3 className="font-semibold mb-2">Student Information</h3>
            <p><strong>Name:</strong> {slip.studentName}</p>
            <p><strong>Student ID:</strong> {slip.studentID}</p>
            <p><strong>Department:</strong> {slip.department}</p>
            <p><strong>Academic Year:</strong> {slip.academicYear}</p>
            <p><strong>Semester:</strong> {slip.semester}</p>
          </div>
          
          {slip.isRegistrarFinalized && (
            <div>
              <h3 className="font-semibold mb-2">Approval Information</h3>
              {slip.serialNumber && (
                <>
                  <p><strong>Serial Number:</strong> {slip.serialNumber}</p>
                  <div className="mt-4">
                    <QRCodeSVG value={slip.serialNumber} size={150} />
                  </div>
                </>
              )}
            </div>
          )}
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
            <div className="flex items-center">
              <span className={slip.isAdvisorApproved ? 'text-green-600' : 'text-gray-400'}>
                {slip.isAdvisorApproved ? '✓' : '○'} Advisor Approved
              </span>
            </div>
            <div className="flex items-center">
              <span className={slip.isCostSharingVerified ? 'text-green-600' : 'text-gray-400'}>
                {slip.isCostSharingVerified ? '✓' : '○'} Cost Sharing Verified
              </span>
            </div>
            <div className="flex items-center">
              <span className={slip.isRegistrarFinalized ? 'text-green-600' : 'text-gray-400'}>
                {slip.isRegistrarFinalized ? '✓' : '○'} Registrar Finalized
              </span>
            </div>
          </div>
        </div>

        {slip.isRegistrarFinalized && (
          <div className="mt-6">
            <a
              href={`/api/student/slips/${slip.id}/pdf`}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
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
