'use client';

import Layout from '@/components/Layout';

export default function DepartmentHeadHome() {
  return (
    <Layout role="DepartmentHead">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Department Head Dashboard</h1>
        <p className="text-gray-600">Welcome to the Department Head dashboard. Use the navigation menu to access different features.</p>
      </div>
    </Layout>
  );
}
