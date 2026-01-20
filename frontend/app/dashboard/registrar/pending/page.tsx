'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import Link from 'next/link';
import { toast } from 'react-toastify';

export default function RegistrarPending() {
  const [slips, setSlips] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchSlips();
  }, []);

  const fetchSlips = async () => {
    try {
      const response = await api.get('/registrar/pending-approvals');
      setSlips(response.data);
    } catch (error) {
      toast.error('Failed to load pending approvals');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout role="Registrar">
        <div>Loading...</div>
      </Layout>
    );
  }

  return (
    <Layout role="Registrar">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Pending Final Approvals</h1>
        
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
                    <p className="text-sm text-gray-600">Credit Hours: {slip.totalCreditHours}</p>
                  </div>
                  <Link
                    href={`/dashboard/registrar/slips/${slip.id}`}
                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                  >
                    Review
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
