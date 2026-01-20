'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import Link from 'next/link';
import { toast } from 'react-toastify';

export default function CostSharingHome() {
  const [pendingForms, setPendingForms] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchPendingForms();
  }, []);

  const fetchPendingForms = async () => {
    try {
      const response = await api.get('/costsharing/pending');
      setPendingForms(response.data);
    } catch (error) {
      toast.error('Failed to load pending forms');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout role="CostSharingOfficer">
        <div>Loading...</div>
      </Layout>
    );
  }

  return (
    <Layout role="CostSharingOfficer">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Cost Sharing Officer Dashboard</h1>
        
        <div className="mb-6">
          <div className="border rounded-lg p-4 bg-blue-50">
            <h3 className="font-semibold text-lg mb-2">Pending Payments</h3>
            <p className="text-3xl font-bold text-blue-600">{pendingForms.length}</p>
          </div>
        </div>

        {pendingForms.length > 0 && (
          <div>
            <h2 className="text-xl font-semibold mb-4">Pending Cost Sharing Forms</h2>
            <div className="space-y-4">
              {pendingForms.map((form) => (
                <div key={form.id} className="border rounded-lg p-4">
                  <div className="flex justify-between items-start">
                    <div>
                      <h3 className="font-semibold">Student ID: {form.studentID}</h3>
                      <p className="text-sm text-gray-600">Submitted: {new Date(form.submittedAt).toLocaleDateString()}</p>
                    </div>
                    <Link
                      href={`/dashboard/cost-sharing/review/${form.id}`}
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
