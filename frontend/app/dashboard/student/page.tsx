'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function StudentHome() {
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const response = await api.get('/student/home');
      setData(response.data);
    } catch (error) {
      toast.error('Failed to load data');
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

  return (
    <Layout role="Student">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Student Dashboard</h1>
        
        <div className="grid md:grid-cols-2 gap-6 mb-6">
          <div className="border rounded-lg p-4">
            <h3 className="font-semibold mb-2">Personal Information</h3>
            <p><strong>Name:</strong> {data?.fullName}</p>
            <p><strong>Student ID:</strong> {data?.studentId}</p>
            <p><strong>Email:</strong> {data?.email}</p>
            <p><strong>Department:</strong> {data?.department}</p>
          </div>
          
          <div className="border rounded-lg p-4">
            <h3 className="font-semibold mb-2">Academic Information</h3>
            <p><strong>Enrollment Year:</strong> {data?.enrollmentYear}</p>
            <p><strong>Academic Year:</strong> Year {data?.academicYear}</p>
            <p><strong>Registration Status:</strong> 
              <span className={`ml-2 px-2 py-1 rounded ${
                data?.registrationStatus === 'RegistrarFinalized' 
                  ? 'bg-green-100 text-green-800' 
                  : 'bg-yellow-100 text-yellow-800'
              }`}>
                {data?.registrationStatus || 'Not Registered'}
              </span>
            </p>
          </div>
        </div>

        {data?.latestSlip && (
          <div className="border rounded-lg p-4">
            <h3 className="font-semibold mb-2">Latest Registration Slip</h3>
            <div className="grid grid-cols-2 gap-4">
              <p><strong>Semester:</strong> {data.latestSlip.semester}</p>
              <p><strong>Total Credit Hours:</strong> {data.latestSlip.totalCreditHours}</p>
              <p><strong>Status:</strong> {data.latestSlip.status}</p>
              <p><strong>Created:</strong> {new Date(data.latestSlip.createdAt).toLocaleDateString()}</p>
            </div>
            {data.latestSlip.isRegistrarFinalized && (
              <div className="mt-4">
                <a
                  href={`/dashboard/student/slips/${data.latestSlip.id}`}
                  className="text-blue-600 hover:underline"
                >
                  View Approved Slip →
                </a>
              </div>
            )}
          </div>
        )}
      </div>
    </Layout>
  );
}
