'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function CostSharingReview() {
  const params = useParams();
  const router = useRouter();
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const response = await api.get(`/costsharing/review/${params.id}`);
      setData(response.data);
    } catch (error) {
      toast.error('Failed to load form');
    } finally {
      setLoading(false);
    }
  };

  const handleVerify = async () => {
    try {
      await api.post(`/costsharing/verify/${params.id}`);
      toast.success('Cost sharing verified successfully');
      router.push('/dashboard/cost-sharing');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Verification failed');
    }
  };

  if (loading) {
    return (
      <Layout role="CostSharingOfficer">
        <div>Loading...</div>
      </Layout>
    );
  }

  if (!data) {
    return (
      <Layout role="CostSharingOfficer">
        <div>Form not found</div>
      </Layout>
    );
  }

  return (
    <Layout role="CostSharingOfficer">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Review Cost Sharing Form</h1>
        
        {data.form && (
          <div className="mb-6">
            <h3 className="font-semibold mb-2">Form Information</h3>
            <p><strong>Student ID:</strong> {data.form.studentID}</p>
            {data.form.fullName && <p><strong>Full Name:</strong> {data.form.fullName}</p>}
            <p><strong>Status:</strong> {data.form.status}</p>
            {(data.form.photoDataUrl || data.form.photoPath) && (
              <div className="mt-3">
                <p className="font-semibold">Photo</p>
                <div className="w-40 aspect-[3/4] border bg-gray-50 overflow-hidden">
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img
                    src={data.form.photoDataUrl || data.form.photoPath}
                    alt="Student photo"
                    className="w-full h-full object-cover"
                  />
                </div>
              </div>
            )}
            {data.form.paymentInfo && (
              <div className="mt-2">
                <p><strong>Payment Information:</strong></p>
                <p className="text-gray-700">{data.form.paymentInfo}</p>
              </div>
            )}
          </div>
        )}

        {data.slip && (
          <div className="mb-6">
            <h3 className="font-semibold mb-2">Registration Slip</h3>
            <p><strong>Semester:</strong> {data.slip.semester}</p>
            <p><strong>Credit Hours:</strong> {data.slip.totalCreditHours}</p>
          </div>
        )}

        {data.form?.status === 'Pending' && (
          <button
            onClick={handleVerify}
            className="bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700"
          >
            Verify Cost Sharing
          </button>
        )}
      </div>
    </Layout>
  );
}
