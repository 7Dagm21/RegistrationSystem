'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import Link from 'next/link';
import { toast } from 'react-toastify';

export default function StudentSlips() {
  const [slips, setSlips] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchSlips();
  }, []);

  const fetchSlips = async () => {
    try {
      const response = await api.get('/student/slips');
      setSlips(response.data);
    } catch (error) {
      toast.error('Failed to load slips');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'RegistrarFinalized':
        return 'bg-green-100 text-green-800';
      case 'CostSharingVerified':
        return 'bg-blue-100 text-blue-800';
      case 'AdvisorApproved':
        return 'bg-yellow-100 text-yellow-800';
      case 'Created':
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-red-100 text-red-800';
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
        <h1 className="text-2xl font-bold mb-6">My Registration Slips</h1>
        
        {slips.length === 0 ? (
          <p className="text-gray-600">No registration slips found.</p>
        ) : (
          <div className="space-y-4">
            {slips.map((slip) => (
              <div key={slip.id} className="border rounded-lg p-4">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold text-lg">Semester: {slip.semester}</h3>
                    <p className="text-sm text-gray-600">Academic Year: {slip.academicYear}</p>
                    <p className="text-sm text-gray-600">Credit Hours: {slip.totalCreditHours}</p>
                    <p className="text-sm text-gray-600">Courses: {slip.courses.length}</p>
                  </div>
                  <div className="text-right">
                    <span className={`px-3 py-1 rounded text-sm ${getStatusColor(slip.status)}`}>
                      {slip.status}
                    </span>
                    <div className="mt-2">
                      <Link
                        href={`/dashboard/student/slips/${slip.id}`}
                        className="text-blue-600 hover:underline text-sm"
                      >
                        View Details →
                      </Link>
                    </div>
                    {slip.isRegistrarFinalized && (
                      <div className="mt-2">
                        <a
                          href={`/api/student/slips/${slip.id}/pdf`}
                          className="text-green-600 hover:underline text-sm"
                          target="_blank"
                        >
                          Download PDF →
                        </a>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
}
