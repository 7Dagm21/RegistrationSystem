'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function RegistrarArchive() {
  const [slips, setSlips] = useState<any[]>([]);
  const [filters, setFilters] = useState({
    studentId: '',
    semester: '',
    department: '',
  });
  const [loading, setLoading] = useState(false);

  const handleSearch = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (filters.studentId) params.append('studentId', filters.studentId);
      if (filters.semester) params.append('semester', filters.semester);
      if (filters.department) params.append('department', filters.department);
      
      const response = await api.get(`/registrar/archive?${params.toString()}`);
      setSlips(response.data);
    } catch (error) {
      toast.error('Failed to load archive');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    handleSearch();
  }, []);

  return (
    <Layout role="Registrar">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Registration Archive</h1>
        
        <div className="mb-6 grid md:grid-cols-3 gap-4">
          <input
            type="text"
            placeholder="Student ID"
            value={filters.studentId}
            onChange={(e) => setFilters({ ...filters, studentId: e.target.value })}
            className="px-3 py-2 border rounded-lg"
          />
          <input
            type="text"
            placeholder="Semester"
            value={filters.semester}
            onChange={(e) => setFilters({ ...filters, semester: e.target.value })}
            className="px-3 py-2 border rounded-lg"
          />
          <input
            type="text"
            placeholder="Department"
            value={filters.department}
            onChange={(e) => setFilters({ ...filters, department: e.target.value })}
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

        {slips.length === 0 ? (
          <p className="text-gray-600">No records found.</p>
        ) : (
          <div className="space-y-4">
            {slips.map((slip) => (
              <div key={slip.id} className="border rounded-lg p-4">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold">{slip.studentName}</h3>
                    <p className="text-sm text-gray-600">Student ID: {slip.studentID}</p>
                    <p className="text-sm text-gray-600">Semester: {slip.semester}</p>
                    <p className="text-sm text-gray-600">Serial Number: {slip.serialNumber}</p>
                  </div>
                  <a
                    href={`/api/registrar/slips/${slip.id}/pdf`}
                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                    target="_blank"
                  >
                    Download PDF
                  </a>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
}
