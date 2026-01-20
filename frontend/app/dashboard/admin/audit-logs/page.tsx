'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function AuditLogs() {
  const [logs, setLogs] = useState<any[]>([]);
  const [filters, setFilters] = useState({
    userId: '',
    role: '',
    startDate: '',
    endDate: '',
  });
  const [loading, setLoading] = useState(false);

  const handleSearch = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (filters.userId) params.append('userId', filters.userId);
      if (filters.role) params.append('role', filters.role);
      if (filters.startDate) params.append('startDate', filters.startDate);
      if (filters.endDate) params.append('endDate', filters.endDate);
      
      const response = await api.get(`/admin/audit-logs?${params.toString()}`);
      setLogs(response.data);
    } catch (error) {
      toast.error('Failed to load audit logs');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    handleSearch();
  }, []);

  return (
    <Layout role="SystemAdmin">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Audit Logs</h1>
        
        <div className="mb-6 grid md:grid-cols-4 gap-4">
          <input
            type="text"
            placeholder="User ID"
            value={filters.userId}
            onChange={(e) => setFilters({ ...filters, userId: e.target.value })}
            className="px-3 py-2 border rounded-lg"
          />
          <input
            type="text"
            placeholder="Role"
            value={filters.role}
            onChange={(e) => setFilters({ ...filters, role: e.target.value })}
            className="px-3 py-2 border rounded-lg"
          />
          <input
            type="date"
            placeholder="Start Date"
            value={filters.startDate}
            onChange={(e) => setFilters({ ...filters, startDate: e.target.value })}
            className="px-3 py-2 border rounded-lg"
          />
          <input
            type="date"
            placeholder="End Date"
            value={filters.endDate}
            onChange={(e) => setFilters({ ...filters, endDate: e.target.value })}
            className="px-3 py-2 border rounded-lg"
          />
        </div>
        
        <button
          onClick={handleSearch}
          disabled={loading}
          className="mb-6 bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
        >
          {loading ? 'Searching...' : 'Search'}
        </button>

        <div className="overflow-x-auto">
          <table className="w-full border-collapse border">
            <thead>
              <tr className="bg-gray-100">
                <th className="border p-2">Timestamp</th>
                <th className="border p-2">User ID</th>
                <th className="border p-2">Role</th>
                <th className="border p-2">Action</th>
                <th className="border p-2">Details</th>
                <th className="border p-2">IP Address</th>
              </tr>
            </thead>
            <tbody>
              {logs.map((log) => (
                <tr key={log.id}>
                  <td className="border p-2">{new Date(log.timestamp).toLocaleString()}</td>
                  <td className="border p-2">{log.userID}</td>
                  <td className="border p-2">{log.userRole}</td>
                  <td className="border p-2">{log.action}</td>
                  <td className="border p-2">{log.details || 'N/A'}</td>
                  <td className="border p-2">{log.ipAddress || 'N/A'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </Layout>
  );
}
