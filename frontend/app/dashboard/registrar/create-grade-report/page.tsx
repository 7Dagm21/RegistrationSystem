'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function CreateGradeReport() {
  const [studentId, setStudentId] = useState('');
  const [student, setStudent] = useState<any>(null);
  const [selectedSlipId, setSelectedSlipId] = useState<number | null>(null);
  const [formData, setFormData] = useState({
    semester: 'I',
    academicYear: '',
    year: 1,
    major: '',
    admissionClassification: 'Regular',
    program: 'Degree',
    registrarRecorderName: '',
    registrarSignedDate: new Date().toISOString().split('T')[0],
    remark: 'X Promoted',
    previousCredit: 0,
    previousGP: 0,
    previousANG: 0,
    courses: [] as Array<{
      courseCode: string;
      courseTitle: string;
      credit: number;
      numberGrade: number;
      letterGrade: string;
      gradePoint: number;
    }>
  });
  const [loading, setLoading] = useState(false);

  const handleSearchStudent = async () => {
    if (!studentId) {
      toast.error('Please enter a student ID');
      return;
    }

    try {
      const response = await api.get(`/registrar/students/search?studentId=${encodeURIComponent(studentId)}`);
      const studentData = response.data;
      
      console.log('Student search response:', studentData);
      console.log('Registration slips:', studentData.registrationSlips);
      
      // Log each slip's courses
      if (studentData.registrationSlips) {
        studentData.registrationSlips.forEach((slip: any, index: number) => {
          console.log(`Slip ${index}:`, {
            id: slip.id || slip.Id,
            courses: slip.courses || slip.Courses,
            coursesLength: (slip.courses || slip.Courses || []).length,
            isApproved: slip.isAdvisorApproved || slip.IsAdvisorApproved,
            semester: slip.semester || slip.Semester
          });
        });
      }
      
      setStudent(studentData);
      setSelectedSlipId(null);
      setFormData(prev => ({
        ...prev,
        major: studentData.department || '',
        year: studentData.academicYear || 1,
        courses: [] // Reset courses
      }));
      toast.success('Student found');
    } catch (error: any) {
      console.error('Search error:', error);
      const errorMessage = error.response?.data?.message || error.response?.data?.title || error.message || 'Student not found';
      toast.error(errorMessage);
      setStudent(null);
      setSelectedSlipId(null);
    }
  };

  const handleSelectSlip = (slip: any) => {
    console.log('Selected slip:', slip);
    console.log('Slip courses:', slip.courses || slip.Courses);
    
    setSelectedSlipId(slip.id || slip.Id);
    
    // Extract semester and academic year from slip
    const semesterStr = slip.semester || slip.Semester || '';
    const semesterMatch = semesterStr.match(/(Fall|Spring|I|II|1|2)/i);
    const semester = semesterMatch ? (semesterMatch[1].toLowerCase().includes('fall') || semesterMatch[1] === 'I' || semesterMatch[1] === '1' ? 'I' : 'II') : 'I';
    
    // Extract academic year from semester string (e.g., "2022/2023 Fall" -> "2022/2023")
    let academicYear = '';
    const yearMatch = semesterStr.match(/(\d{4}\/\d{4})/);
    if (yearMatch) {
      academicYear = yearMatch[1];
    } else if (semesterStr) {
      // If no year pattern found, use the semester string as-is
      academicYear = semesterStr;
    }
    
    // Populate courses from slip - handle both camelCase and PascalCase
    const coursesArray = slip.courses || slip.Courses || [];
    
    console.log('Courses array:', coursesArray);
    console.log('Is array?', Array.isArray(coursesArray));
    console.log('Length:', coursesArray.length);
    
    if (!Array.isArray(coursesArray) || coursesArray.length === 0) {
      toast.error('No courses found in this registration slip. The slip may need to be created with courses first.');
      setFormData(prev => ({
        ...prev,
        semester: semester,
        academicYear: academicYear || prev.academicYear,
        year: slip.academicYear || slip.AcademicYear || prev.year,
        courses: []
      }));
      return;
    }
    
    const courses = coursesArray.map((course: any, index: number) => {
      // Handle both camelCase and PascalCase property names
      const courseCode = course.courseCode || course.CourseCode || '';
      const courseName = course.courseName || course.CourseName || '';
      const creditHours = course.creditHours || course.CreditHours || 0;
      
      console.log(`Course ${index}:`, { courseCode, courseName, creditHours, fullCourse: course });
      
      return {
        courseCode: courseCode,
        courseTitle: courseName,
        credit: creditHours,
        numberGrade: 0,
        letterGrade: '',
        gradePoint: 0
      };
    });

    console.log('Mapped courses:', courses);

    setFormData(prev => ({
      ...prev,
      semester: semester,
      academicYear: academicYear || prev.academicYear,
      year: slip.academicYear || slip.AcademicYear || prev.year,
      courses: courses
    }));

    toast.success(`Loaded ${courses.length} courses from registration slip`);
  };

  // Courses are auto-populated from registration slip, cannot add/remove

  // Convert number grade to letter grade based on AASTU grading scale
  const getLetterGrade = (numberGrade: number): string => {
    if (numberGrade >= 90 && numberGrade <= 100) return 'A+';
    if (numberGrade >= 85 && numberGrade <= 89) return 'A';
    if (numberGrade >= 80 && numberGrade <= 84) return 'A-';
    if (numberGrade >= 75 && numberGrade <= 79) return 'B+';
    if (numberGrade >= 70 && numberGrade <= 74) return 'B';
    if (numberGrade >= 65 && numberGrade <= 69) return 'B-';
    if (numberGrade >= 60 && numberGrade <= 64) return 'C+';
    if (numberGrade >= 50 && numberGrade <= 59) return 'C';
    if (numberGrade >= 45 && numberGrade <= 49) return 'C-';
    if (numberGrade >= 40 && numberGrade <= 44) return 'D';
    if (numberGrade >= 35 && numberGrade <= 39) return 'Fx';
    if (numberGrade >= 0 && numberGrade <= 34) return 'F';
    return '';
  };

  // Get grade point value from letter grade
  const getGradePointValue = (letterGrade: string): number => {
    const gradeMap: Record<string, number> = {
      'A+': 4.0,
      'A': 4.0,
      'A-': 3.75,
      'B+': 3.5,
      'B': 3.0,
      'B-': 2.75,
      'C+': 2.5,
      'C': 2.0,
      'C-': 1.75,
      'D': 1.0,
      'Fx': 0.0,
      'F': 0.0
    };
    return gradeMap[letterGrade.toUpperCase()] || 0;
  };

  const updateCourse = (index: number, field: string, value: any) => {
    setFormData(prev => {
      const newCourses = [...prev.courses];
      newCourses[index] = { ...newCourses[index], [field]: value };
      
      // Auto-calculate letter grade and grade point when number grade changes
      if (field === 'numberGrade') {
        const numGrade = parseFloat(value) || 0;
        const letterGrade = getLetterGrade(numGrade);
        const gradePointValue = getGradePointValue(letterGrade);
        const credit = newCourses[index].credit;
        
        newCourses[index].letterGrade = letterGrade;
        // GradePoint = CreditHours × GradePointValue (not NumberGrade × Credit)
        newCourses[index].gradePoint = credit * gradePointValue;
      }
      
      // If letter grade is manually changed, update grade point
      if (field === 'letterGrade') {
        const gradePointValue = getGradePointValue(value);
        const credit = newCourses[index].credit;
        newCourses[index].gradePoint = credit * gradePointValue;
      }
      
      // If credit changes, recalculate grade point
      if (field === 'credit') {
        const letterGrade = newCourses[index].letterGrade || getLetterGrade(newCourses[index].numberGrade || 0);
        const gradePointValue = getGradePointValue(letterGrade);
        newCourses[index].gradePoint = value * gradePointValue;
      }
      
      return { ...prev, courses: newCourses };
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!student) {
      toast.error('Please search and select a student first');
      return;
    }

    if (!selectedSlipId) {
      toast.error('Please select a registration slip first');
      return;
    }

    if (formData.courses.length === 0) {
      toast.error('No courses found in the selected slip');
      return;
    }

    // Validate all courses have grades
    const incompleteCourses = formData.courses.filter(c => !c.numberGrade || !c.letterGrade);
    if (incompleteCourses.length > 0) {
      toast.error(`Please enter grades for all ${incompleteCourses.length} course(s)`);
      return;
    }

    // Validate academic year format
    if (!formData.academicYear || formData.academicYear.trim() === '') {
      toast.error('Please enter the academic year (e.g., 2022/2023)');
      return;
    }

    setLoading(true);
    try {
      // Map courses to backend format (matching CourseGradeDto)
      const coursesForBackend = formData.courses.map(c => {
        // Ensure grade point is calculated correctly
        const gradePointValue = getGradePointValue(c.letterGrade);
        const calculatedGradePoint = c.credit * gradePointValue;
        
        return {
          CourseCode: c.courseCode,
          CourseTitle: c.courseTitle,
          Credit: c.credit,
          NumberGrade: c.numberGrade,
          LetterGrade: c.letterGrade,
          GradePoint: calculatedGradePoint // Ensure it's Credit × GradePointValue
        };
      });

      const requestData = {
        StudentID: studentId,
        Semester: formData.semester,
        AcademicYear: formData.academicYear.trim(),
        Year: formData.year,
        Major: formData.major || student.department,
        AdmissionClassification: formData.admissionClassification,
        Program: formData.program,
        Courses: coursesForBackend,
        PreviousCredit: formData.previousCredit || 0,
        PreviousGP: formData.previousGP || 0,
        PreviousANG: formData.previousANG || 0,
        RegistrarRecorderName: formData.registrarRecorderName,
        RegistrarSignedDate: formData.registrarSignedDate,
        Remark: formData.remark
      };

      console.log('Sending grade report request:', requestData);

      const response = await api.post('/gradereport/create', requestData);

      // Get GPA from response
      const gpa = response.data?.gpa ?? 0;
      const cgpa = response.data?.cgpa ?? 0;
      
      toast.success(
        `Grade report created successfully! GPA: ${gpa.toFixed(2)}, CGPA: ${cgpa.toFixed(2)}`,
        { autoClose: 5000 }
      );
      
      // Reset form
      setStudent(null);
      setStudentId('');
      setSelectedSlipId(null);
      setFormData({
        semester: 'I',
        academicYear: '',
        year: 1,
        major: '',
        admissionClassification: 'Regular',
        program: 'Degree',
        registrarRecorderName: '',
        registrarSignedDate: new Date().toISOString().split('T')[0],
        remark: 'X Promoted',
        previousCredit: 0,
        previousGP: 0,
        previousANG: 0,
        courses: []
      });
    } catch (error: any) {
      console.error('Error creating grade report:', error);
      const errorMessage = error.response?.data?.message || 
                          error.response?.data?.title || 
                          error.message || 
                          'Failed to create grade report';
      toast.error(`Error: ${errorMessage}`);
      
      // Show detailed error if available
      if (error.response?.data) {
        console.error('Error details:', error.response.data);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout role="Registrar">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Create Grade Report</h1>

        {/* Student Search */}
        <div className="mb-6">
          <label className="block text-sm font-medium mb-2">Student ID</label>
          <div className="flex gap-2">
            <input
              type="text"
              value={studentId}
              onChange={(e) => setStudentId(e.target.value)}
              className="flex-1 px-3 py-2 border rounded-lg"
              placeholder="Enter Student ID (e.g., ETS0410/15)"
            />
            <button
              onClick={handleSearchStudent}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              Search
            </button>
          </div>
          {student && (
            <div className="mt-4 space-y-4">
              <div className="p-4 bg-green-50 rounded">
                <p><strong>Name:</strong> {student.fullName}</p>
                <p><strong>Department:</strong> {student.department}</p>
                <p><strong>Academic Year:</strong> {student.academicYear}</p>
              </div>
              
              {student.registrationSlips && student.registrationSlips.length > 0 ? (
                <div>
                  <h3 className="font-semibold mb-2">Select Registration Slip to Grade:</h3>
                  {student.approvedSlips === 0 && (
                    <div className="mb-3 p-3 bg-yellow-50 border border-yellow-200 rounded">
                      <p className="text-sm text-yellow-800">
                        ⚠️ No approved slips found. Only advisor-approved slips can be used for grading.
                      </p>
                    </div>
                  )}
                  <div className="space-y-2">
                    {student.registrationSlips.map((slip: any) => {
                      const coursesArray = slip.courses || slip.Courses || [];
                      const isApproved = slip.isAdvisorApproved || slip.IsAdvisorApproved || false;
                      const canGrade = isApproved && Array.isArray(coursesArray) && coursesArray.length > 0;
                      return (
                        <div
                          key={slip.id}
                          className={`border rounded-lg p-3 transition ${
                            canGrade
                              ? selectedSlipId === slip.id
                                ? 'bg-blue-100 border-blue-500 cursor-pointer'
                                : 'bg-white hover:bg-gray-50 cursor-pointer'
                              : 'bg-gray-100 border-gray-300 opacity-60 cursor-not-allowed'
                          }`}
                          onClick={() => canGrade && handleSelectSlip(slip)}
                        >
                          <div className="flex justify-between items-start">
                            <div>
                              <p className="font-medium">{slip.semester}</p>
                              <p className="text-sm text-gray-600">
                                {slip.courses?.length || 0} courses • {slip.totalCreditHours} credit hours
                              </p>
                              <div className="flex gap-2 mt-1 flex-wrap">
                                {slip.isRegistrarFinalized && (
                                  <span className="text-xs bg-green-100 text-green-800 px-2 py-1 rounded">✓ Finalized</span>
                                )}
                                {slip.isCostSharingVerified && !slip.isRegistrarFinalized && (
                                  <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">✓ Cost-Sharing Verified</span>
                                )}
                                {slip.isAdvisorApproved && (
                                  <span className="text-xs bg-yellow-100 text-yellow-800 px-2 py-1 rounded">✓ Advisor Approved</span>
                                )}
                                {!slip.isAdvisorApproved && (
                                  <span className="text-xs bg-red-100 text-red-800 px-2 py-1 rounded">⚠ Not Approved</span>
                                )}
                              {(!slip.courses && !slip.Courses || (slip.courses || slip.Courses || []).length === 0) && (
                                <span className="text-xs bg-gray-100 text-gray-800 px-2 py-1 rounded">No Courses</span>
                              )}
                              </div>
                            <p className="text-xs text-gray-500 mt-1">
                              Created: {slip.createdAt ? new Date(slip.createdAt).toLocaleDateString() : (slip.CreatedAt ? new Date(slip.CreatedAt).toLocaleDateString() : 'N/A')}
                            </p>
                              {!canGrade && (
                                <p className="text-xs text-red-600 mt-1">
                                  Cannot grade: {!(slip.isAdvisorApproved || slip.IsAdvisorApproved) ? 'Not approved by advisor' : 'No courses'}
                                </p>
                              )}
                            </div>
                            {selectedSlipId === slip.id && canGrade && (
                              <span className="text-blue-600 font-semibold">✓ Selected</span>
                            )}
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>
              ) : (
                <div className="p-4 bg-yellow-50 rounded">
                  <p className="text-yellow-800">
                    No registration slips found for this student. 
                    The student must have at least an advisor-approved registration slip before grades can be assigned.
                  </p>
                </div>
              )}
            </div>
          )}
        </div>

        {student && selectedSlipId && formData.courses.length > 0 && (
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Basic Information */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-2">Semester</label>
                <select
                  value={formData.semester}
                  onChange={(e) => setFormData({ ...formData, semester: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                >
                  <option value="I">I</option>
                  <option value="II">II</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Academic Year</label>
                <input
                  type="text"
                  value={formData.academicYear}
                  onChange={(e) => setFormData({ ...formData, academicYear: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  placeholder="e.g., 2022/2023"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Year</label>
                <input
                  type="number"
                  value={formData.year}
                  onChange={(e) => setFormData({ ...formData, year: parseInt(e.target.value) })}
                  className="w-full px-3 py-2 border rounded-lg"
                  min="1"
                  max="5"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Major</label>
                <input
                  type="text"
                  value={formData.major}
                  onChange={(e) => setFormData({ ...formData, major: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                />
              </div>
            </div>

            {/* Previous Totals */}
            <div>
              <h3 className="font-semibold mb-3">Previous Totals</h3>
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-2">Previous Credit</label>
                  <input
                    type="number"
                    step="0.01"
                    value={formData.previousCredit}
                    onChange={(e) => setFormData({ ...formData, previousCredit: parseFloat(e.target.value) || 0 })}
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-2">Previous GP</label>
                  <input
                    type="number"
                    step="0.01"
                    value={formData.previousGP}
                    onChange={(e) => setFormData({ ...formData, previousGP: parseFloat(e.target.value) || 0 })}
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-2">Previous ANG</label>
                  <input
                    type="number"
                    step="0.01"
                    value={formData.previousANG}
                    onChange={(e) => setFormData({ ...formData, previousANG: parseFloat(e.target.value) || 0 })}
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                </div>
              </div>
            </div>

            {/* Courses */}
            {formData.courses.length > 0 && (
              <div>
                <div className="mb-3">
                  <h3 className="font-semibold">Courses (from Registration Slip)</h3>
                  <p className="text-sm text-gray-600">
                    Courses are loaded from the selected registration slip. Only grades can be entered.
                  </p>
                </div>
                <div className="space-y-4">
                  {formData.courses.map((course, index) => (
                    <div key={index} className="border rounded-lg p-4 bg-gray-50">
                      <div className="grid grid-cols-6 gap-4">
                        <div>
                          <label className="block text-xs font-medium mb-1">Course Code</label>
                          <input
                            type="text"
                            value={course.courseCode}
                            className="w-full px-2 py-1 border rounded text-sm bg-white"
                            readOnly
                          />
                        </div>
                        <div className="col-span-2">
                          <label className="block text-xs font-medium mb-1">Course Title</label>
                          <input
                            type="text"
                            value={course.courseTitle}
                            className="w-full px-2 py-1 border rounded text-sm bg-white"
                            readOnly
                          />
                        </div>
                        <div>
                          <label className="block text-xs font-medium mb-1">Credit</label>
                          <input
                            type="number"
                            value={course.credit}
                            className="w-full px-2 py-1 border rounded text-sm bg-white"
                            readOnly
                          />
                        </div>
                        <div>
                          <label className="block text-xs font-medium mb-1">Number Grade</label>
                          <input
                            type="number"
                            step="0.01"
                            min="0"
                            max="100"
                            value={course.numberGrade || ''}
                            onChange={(e) => updateCourse(index, 'numberGrade', parseFloat(e.target.value) || 0)}
                            className="w-full px-2 py-1 border rounded text-sm bg-white"
                            placeholder="0-100"
                            required
                          />
                        </div>
                        <div>
                          <label className="block text-xs font-medium mb-1">Letter Grade</label>
                          <input
                            type="text"
                            value={course.letterGrade}
                            onChange={(e) => updateCourse(index, 'letterGrade', e.target.value.toUpperCase())}
                            className="w-full px-2 py-1 border rounded text-sm bg-white"
                            placeholder="Auto-filled from number grade"
                            required
                            readOnly={!!course.numberGrade}
                          />
                        </div>
                      </div>
                      <div className="mt-2 flex justify-between items-center">
                        <span className="text-sm text-gray-600">
                          Grade Point Value: <strong>{course.letterGrade ? getGradePointValue(course.letterGrade).toFixed(2) : '0.00'}</strong>
                        </span>
                        <span className="text-sm font-semibold text-blue-600">
                          Total Points: <strong>{course.gradePoint.toFixed(2)}</strong> (Credit × Grade Point)
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
            
            {selectedSlipId && formData.courses.length === 0 && (
              <div className="p-4 bg-yellow-50 rounded">
                <p className="text-yellow-800">
                  No courses found in the selected registration slip.
                </p>
              </div>
            )}

            {/* Registrar Info */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-2">Registrar Recorder Name</label>
                <input
                  type="text"
                  value={formData.registrarRecorderName}
                  onChange={(e) => setFormData({ ...formData, registrarRecorderName: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Date</label>
                <input
                  type="date"
                  value={formData.registrarSignedDate}
                  onChange={(e) => setFormData({ ...formData, registrarSignedDate: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">Remark</label>
              <select
                value={formData.remark}
                onChange={(e) => setFormData({ ...formData, remark: e.target.value })}
                className="w-full px-3 py-2 border rounded-lg"
              >
                <option value="X Promoted">X Promoted</option>
                <option value="Academic Warning">Academic Warning</option>
                <option value="Academic Dismissal">Academic Dismissal</option>
                <option value="Withdraw for Academic Reason">Withdraw for Academic Reason</option>
                <option value="Complete Academic Dismissal">Complete Academic Dismissal</option>
                <option value="Graduation Approved">Graduation Approved</option>
              </select>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:bg-gray-400"
            >
              {loading ? 'Creating...' : 'Create Grade Report'}
            </button>
          </form>
        )}
      </div>
    </Layout>
  );
}
