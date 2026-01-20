'use client';

import Link from 'next/link';
import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { getAuth } from '@/lib/auth';

export default function Home() {
  const router = useRouter();
  const user = getAuth();

  useEffect(() => {
    if (user) {
      // Redirect based on role
      const role = user.role;
      if (role === 'Student') router.push('/dashboard/student');
      else if (role === 'Advisor') router.push('/dashboard/advisor');
      else if (role === 'DepartmentHead') router.push('/dashboard/department-head');
      else if (role === 'Registrar') router.push('/dashboard/registrar');
      else if (role === 'CostSharingOfficer') router.push('/dashboard/cost-sharing');
      else if (role === 'SystemAdmin') router.push('/dashboard/admin');
    }
  }, [user, router]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      <div className="container mx-auto px-4 py-16">
        <div className="max-w-4xl mx-auto text-center">
          <h1 className="text-5xl font-bold text-gray-900 mb-6">
            AASTU Registration System
          </h1>
          <p className="text-xl text-gray-700 mb-8">
            Digital Transformation of University Registration Workflow
          </p>
          <div className="bg-yellow-100 border-l-4 border-yellow-500 text-yellow-700 p-4 mb-8 rounded">
            <p className="font-semibold">⚠️ Accessible only within AASTU network</p>
          </div>
          
          <div className="flex gap-4 justify-center">
            <Link
              href="/signin"
              className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-8 rounded-lg transition"
            >
              Sign In
            </Link>
            <Link
              href="/signup"
              className="bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-8 rounded-lg transition"
            >
              Sign Up
            </Link>
          </div>

          <div className="mt-16 grid md:grid-cols-3 gap-8 text-left">
            <div className="bg-white p-6 rounded-lg shadow">
              <h3 className="text-xl font-semibold mb-3">For Students</h3>
              <p className="text-gray-600">
                View registration status, download approved slips, submit cost sharing forms, and track your academic progress.
              </p>
            </div>
            <div className="bg-white p-6 rounded-lg shadow">
              <h3 className="text-xl font-semibold mb-3">For Staff</h3>
              <p className="text-gray-600">
                Manage student registrations, review and approve slips, verify payments, and finalize registrations.
              </p>
            </div>
            <div className="bg-white p-6 rounded-lg shadow">
              <h3 className="text-xl font-semibold mb-3">Secure & Efficient</h3>
              <p className="text-gray-600">
                Role-based access control, email verification, network restrictions, and complete audit logging.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
