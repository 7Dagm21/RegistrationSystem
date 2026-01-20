'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import Link from 'next/link';
import { toast } from 'react-toastify';

export default function AdvisorPending() {
  const [slips, setSlips] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchSlips();
  }, []);

  const fetchSlips = async () => {
    try {
      const response = await api.get('/advisor/pending-slips');
      setSlips(response.data);
    } catch (error) {
      toast.error('Failed to load slips');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout role="Advisor">
        <div>Loading...</div>
      </Layout>
    );
  }

  return (
    <Layout role="Advisor">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Pending Slip Approvals</h1>
        
        {slips.length === 0 ? (
          <p className="text-gray-600">No pending approvals.</p>
        ) : (
          <div className="space-y-4">
            {slips.map((slip) => (
              <div key={slip.id} className="border rounded-lg p-4">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold text-lg">{slip.studentName}</h3>
                    <p className="text-sm text-gray-600">Student ID: {slip.studentID}</p>
                    <p className="text-sm text-gray-600">Department: {slip.department}</p>
                    <p className="text-sm text-gray-600">Semester: {slip.semester}</p>
                    <p className="text-sm text-gray-600">Academic Year: {slip.academicYear}</p>
                    <p className="text-sm text-gray-600">Total Credit Hours: {slip.totalCreditHours}</p>
                    <p className="text-sm text-gray-600">Courses: {slip.courses.length}</p>
                  </div>
                  <Link
                    href={`/dashboard/advisor/slips/${slip.id}`}
                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                  >
                    Review Slip
                  </Link>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
}
