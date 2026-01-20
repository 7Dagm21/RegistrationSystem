'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import Link from 'next/link';
import { toast } from 'react-toastify';

export default function RegistrarHome() {
  const [pendingSlips, setPendingSlips] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchPendingSlips();
  }, []);

  const fetchPendingSlips = async () => {
    try {
      const response = await api.get('/registrar/pending-approvals');
      setPendingSlips(response.data);
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
        <h1 className="text-2xl font-bold mb-6">Registrar Dashboard</h1>
        
        <div className="mb-6">
          <div className="border rounded-lg p-4 bg-blue-50">
            <h3 className="font-semibold text-lg mb-2">Pending Final Approvals</h3>
            <p className="text-3xl font-bold text-blue-600">{pendingSlips.length}</p>
          </div>
        </div>

        {pendingSlips.length > 0 && (
          <div>
            <h2 className="text-xl font-semibold mb-4">Pending Registrations</h2>
            <div className="space-y-4">
              {pendingSlips.slice(0, 5).map((slip) => (
                <div key={slip.id} className="border rounded-lg p-4">
                  <div className="flex justify-between items-start">
                    <div>
                      <h3 className="font-semibold">{slip.studentName}</h3>
                      <p className="text-sm text-gray-600">Student ID: {slip.studentID}</p>
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
          </div>
        )}
      </div>
    </Layout>
  );
}
