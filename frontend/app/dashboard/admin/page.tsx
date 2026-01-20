'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import Link from 'next/link';
import api from '@/lib/api';

export default function AdminHome() {
  const [stats, setStats] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      const [studentsRes, staffRes, usersRes] = await Promise.all([
        api.get('/admin/students'),
        api.get('/admin/staff'),
        api.get('/admin/users'),
      ]);
      setStats({
        students: studentsRes.data.length,
        staff: staffRes.data.length,
        users: usersRes.data.length,
      });
    } catch (error) {
      console.error('Failed to load stats');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout role="SystemAdmin">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">System Admin Dashboard</h1>
        
        {loading ? (
          <p>Loading...</p>
        ) : (
          <div className="grid md:grid-cols-3 gap-6 mb-8">
            <div className="border rounded-lg p-4 bg-blue-50">
              <h3 className="font-semibold text-lg mb-2">Base Students</h3>
              <p className="text-3xl font-bold text-blue-600">{stats?.students || 0}</p>
              <Link href="/dashboard/admin/students" className="text-blue-600 hover:underline text-sm mt-2 inline-block">
                Manage →
              </Link>
            </div>
            
            <div className="border rounded-lg p-4 bg-green-50">
              <h3 className="font-semibold text-lg mb-2">Base Staff</h3>
              <p className="text-3xl font-bold text-green-600">{stats?.staff || 0}</p>
              <Link href="/dashboard/admin/staff" className="text-green-600 hover:underline text-sm mt-2 inline-block">
                Manage →
              </Link>
            </div>
            
            <div className="border rounded-lg p-4 bg-purple-50">
              <h3 className="font-semibold text-lg mb-2">System Users</h3>
              <p className="text-3xl font-bold text-purple-600">{stats?.users || 0}</p>
              <Link href="/dashboard/admin/users" className="text-purple-600 hover:underline text-sm mt-2 inline-block">
                Manage →
              </Link>
            </div>
          </div>
        )}

        <div className="mb-6 p-4 bg-yellow-50 border border-yellow-200 rounded">
          <h3 className="font-semibold mb-2">Important: Base Database</h3>
          <p className="text-sm text-yellow-800">
            The <strong>Students</strong> and <strong>Staff</strong> tables are the base database. 
            Only people in these tables can sign up for accounts. This is how the system validates 
            if someone is part of the university.
          </p>
        </div>

        <div className="grid md:grid-cols-2 gap-6">
          <div className="border rounded-lg p-4">
            <h3 className="font-semibold mb-3">Quick Actions</h3>
            <ul className="space-y-2">
              <li>
                <Link href="/dashboard/admin/students" className="text-blue-600 hover:underline">
                  Manage Students (Base Database)
                </Link>
              </li>
              <li>
                <Link href="/dashboard/admin/staff" className="text-blue-600 hover:underline">
                  Manage Staff (Base Database)
                </Link>
              </li>
              <li>
                <Link href="/dashboard/admin/users" className="text-blue-600 hover:underline">
                  Manage System Users
                </Link>
              </li>
              <li>
                <Link href="/dashboard/admin/audit-logs" className="text-blue-600 hover:underline">
                  View Audit Logs
                </Link>
              </li>
            </ul>
          </div>

          <div className="border rounded-lg p-4">
            <h3 className="font-semibold mb-3">Information</h3>
            <ul className="space-y-2 text-sm text-gray-600">
              <li>• Base Students: Pre-registered students who can sign up</li>
              <li>• Base Staff: Pre-registered staff who can sign up</li>
              <li>• System Users: People who have created accounts</li>
              <li>• Only base database members can create accounts</li>
            </ul>
          </div>
        </div>
      </div>
    </Layout>
  );
}
