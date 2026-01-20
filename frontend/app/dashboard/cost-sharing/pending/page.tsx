'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import Link from 'next/link';
import { toast } from 'react-toastify';

export default function CostSharingPending() {
  const [forms, setForms] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchForms();
  }, []);

  const fetchForms = async () => {
    try {
      const response = await api.get('/costsharing/pending');
      setForms(response.data);
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
        <h1 className="text-2xl font-bold mb-6">Pending Cost Sharing Forms</h1>
        
        {forms.length === 0 ? (
          <p className="text-gray-600">No pending forms.</p>
        ) : (
          <div className="space-y-4">
            {forms.map((form) => (
              <div key={form.id} className="border rounded-lg p-4">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold">Student ID: {form.studentID}</h3>
                    <p className="text-sm text-gray-600">Submitted: {new Date(form.submittedAt).toLocaleDateString()}</p>
                    {form.paymentInfo && (
                      <p className="text-sm text-gray-600 mt-2">{form.paymentInfo}</p>
                    )}
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
        )}
      </div>
    </Layout>
  );
}
