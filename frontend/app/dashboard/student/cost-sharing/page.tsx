"use client";

import { useEffect, useState } from "react";
import Layout from "@/components/Layout";
import api from "@/lib/api";
import { toast } from "react-toastify";

export default function CostSharingPage() {
  const [slips, setSlips] = useState<any[]>([]);
  const [selectedSlip, setSelectedSlip] = useState<any>(null);
  const [photoPreview, setPhotoPreview] = useState<string>("");
  const [forms, setForms] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const COSTS = {
    tuitionFee15Percent: 1382.11,
    foodExpense: 22980.0,
    boardingExpense: 600.0,
    totalCost: 24962.11,
  };
  const [formData, setFormData] = useState<any>({
    // photo
    photoDataUrl: "",
    photoPath: "",

    // key form fields
    fullName: "",
    identityNo: "",
    sex: "Female",
    nationality: "Ethiopian",
    dateOfBirth: "",
    placeOfBirth: "",
    mothersFullName: "",
    mothersAddress: "",
    schoolName: "",
    dateCompleted: "",
    facultyOrCollege: "",
    department: "",
    entranceYearEC: "",
    academicYearText: "",
    semesterText: "",

    // service selection
    serviceInKind: "Boarding only",
    serviceInCash: "Food only",

    // costs (STATIC as requested)
    tuitionFee15Percent: 1382.11,
    foodExpense: 22980.0,
    boardingExpense: 600.0,
    totalCost: 24962.11,

    // payment & signatures
    advancePaymentDate: "",
    discount: "",
    receiptNo: "",
    paymentInfo: "",
    beneficiarySignatureName: "",
    beneficiarySignedAt: "",
  });

  useEffect(() => {
    fetchSlips();
    fetchForms();
  }, []);

  const fetchSlips = async () => {
    try {
      const response = await api.get("/student/slips");
      const approvedSlips = response.data.filter(
        (s: any) => s.isAdvisorApproved && !s.isCostSharingVerified,
      );
      setSlips(approvedSlips);
    } catch (error) {
      toast.error("Failed to load slips");
    } finally {
      setLoading(false);
    }
  };

  const fetchForms = async () => {
    setLoading(true);
    try {
      const response = await api.get("/student/cost-sharing-forms");
      setForms(response.data);
    } catch (error: any) {
      toast.error(
        error.response?.data?.message || "Failed to load cost sharing forms",
      );
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedSlip) {
      toast.error("Please select a slip");
      return;
    }

    try {
      const serviceSelection = JSON.stringify({
        inKind: formData.serviceInKind,
        inCash: formData.serviceInCash,
      });

      await api.post("/costsharing/submit", {
        slipId: selectedSlip.id,
        photoPath: formData.photoPath || null,
        photoDataUrl: formData.photoDataUrl || null,
        paymentInfo: formData.paymentInfo || null,

        fullName: formData.fullName || null,
        identityNo: formData.identityNo || null,
        sex: formData.sex || null,
        nationality: formData.nationality || null,
        dateOfBirth: formData.dateOfBirth
          ? new Date(formData.dateOfBirth).toISOString()
          : null,
        placeOfBirth: formData.placeOfBirth || null,
        mothersFullName: formData.mothersFullName || null,
        mothersAddress: formData.mothersAddress || null,
        schoolName: formData.schoolName || null,
        dateCompleted: formData.dateCompleted
          ? new Date(formData.dateCompleted).toISOString()
          : null,
        facultyOrCollege: formData.facultyOrCollege || null,
        department: formData.department || selectedSlip.department || null,
        entranceYearEC: formData.entranceYearEC || null,
        academicYearText: formData.academicYearText || null,
        semesterText: formData.semesterText || null,

        serviceSelection,
        // Section 11 is optional
        advancePaymentDate: formData.advancePaymentDate
          ? new Date(formData.advancePaymentDate).toISOString()
          : null,
        discount: formData.discount || null,
        receiptNo: formData.receiptNo || null,
        beneficiarySignatureName: formData.beneficiarySignatureName || null,
        beneficiarySignedAt: formData.beneficiarySignedAt
          ? new Date(formData.beneficiarySignedAt).toISOString()
          : null,
      });
      toast.success("Cost sharing form submitted successfully");
      fetchSlips();
      setSelectedSlip(null);
      setPhotoPreview("");
      setFormData((prev: any) => ({
        ...prev,
        photoDataUrl: "",
        photoPath: "",
        paymentInfo: "",
        fullName: "",
        identityNo: "",
        sex: "Female",
        nationality: "Ethiopian",
        dateOfBirth: "",
        placeOfBirth: "",
        mothersFullName: "",
        mothersAddress: "",
        schoolName: "",
        dateCompleted: "",
        facultyOrCollege: "",
        department: "",
        entranceYearEC: "",
        academicYearText: "",
        semesterText: "",
        serviceInKind: "Boarding only",
        serviceInCash: "Food only",
        advancePaymentDate: "",
        discount: "",
        receiptNo: "",
        beneficiarySignatureName: "",
        beneficiarySignedAt: "",
      }));
    } catch (error: any) {
      toast.error(error.response?.data?.message || "Submission failed");
    }
  };

  const onPhotoFileChange = (file: File | null) => {
    if (!file) return;
    if (!file.type.startsWith("image/")) {
      toast.error("Please select an image file");
      return;
    }
    if (file.size > 2 * 1024 * 1024) {
      toast.error("Image too large (max 2MB)");
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      const dataUrl = String(reader.result || "");
      setPhotoPreview(dataUrl);
      setFormData((prev: any) => ({ ...prev, photoDataUrl: dataUrl }));
    };
    reader.readAsDataURL(file);
  };

  const handleDownload = async (id: number) => {
    try {
      const response = await api.get(
        `/student/slips/${id}/cost-sharing-form/pdf`,
        { responseType: "blob" },
      );
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `CostSharingForm_${id}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (error: any) {
      toast.error("Failed to download PDF");
    }
  };

  if (loading) {
    return (
      <Layout role="Student">
        <div>Loading...</div>
      </Layout>
    );
  }

  return (
    <Layout role="Student">
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex items-start justify-between gap-4 mb-6">
          <div>
            <h1 className="text-2xl font-bold">
              Cost-Sharing Beneficiaries Agreement Form
            </h1>
            <p className="text-sm text-gray-600">
              Digital form (styled like the paper form)
            </p>
          </div>
          <div className="text-right text-xs text-gray-600">
            <div className="font-semibold">VPAA/REG/OF/013</div>
            <div>Page 1 of 3</div>
          </div>
        </div>

        {slips.length === 0 ? (
          <p className="text-gray-600">
            No slips pending cost sharing submission.
          </p>
        ) : (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-2">
                Select Slip
              </label>
              <select
                value={selectedSlip?.id || ""}
                onChange={(e) => {
                  const slip = slips.find(
                    (s) => s.id === parseInt(e.target.value),
                  );
                  setSelectedSlip(slip);
                }}
                className="w-full px-3 py-2 border rounded-lg"
              >
                <option value="">Select a slip</option>
                {slips.map((slip) => (
                  <option key={slip.id} value={slip.id}>
                    {slip.semester} - {slip.totalCreditHours} Credit Hours
                  </option>
                ))}
              </select>
            </div>

            {selectedSlip && (
              <form onSubmit={handleSubmit} className="space-y-6">
                {/* Header box like the paper form */}
                <div className="border-2 border-gray-800 p-4">
                  <div className="text-center font-semibold">
                    ADDIS ABABA SCIENCE AND TECHNOLOGY UNIVERSITY
                  </div>
                  <div className="text-center font-bold mt-1">
                    COST-SHARING BENEFICIARIES AGREEMENT FORM
                  </div>
                </div>

                {/* Section 1 + photo */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4 border border-gray-800 p-4">
                  <div className="md:col-span-2 space-y-3">
                    <div className="text-sm font-semibold">
                      1. Full Name (including grand father&apos;s name)
                    </div>
                    <input
                      className="w-full border-b border-gray-700 outline-none px-1 py-1"
                      value={formData.fullName}
                      onChange={(e) =>
                        setFormData({ ...formData, fullName: e.target.value })
                      }
                      placeholder="Full name"
                      required
                    />

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                      <div>
                        <div className="text-sm font-semibold">
                          Identity No.
                        </div>
                        <input
                          className="w-full border-b border-gray-700 outline-none px-1 py-1"
                          value={formData.identityNo}
                          onChange={(e) =>
                            setFormData({
                              ...formData,
                              identityNo: e.target.value,
                            })
                          }
                          placeholder="e.g. ETS.... or Student ID"
                        />
                      </div>
                      <div>
                        <div className="text-sm font-semibold">Nationality</div>
                        <input
                          className="w-full border-b border-gray-700 outline-none px-1 py-1"
                          value={formData.nationality}
                          onChange={(e) =>
                            setFormData({
                              ...formData,
                              nationality: e.target.value,
                            })
                          }
                        />
                      </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                      <div>
                        <div className="text-sm font-semibold">2. Sex</div>
                        <div className="flex gap-4 items-center">
                          <label className="flex items-center gap-2 text-sm">
                            <input
                              type="radio"
                              name="sex"
                              checked={formData.sex === "Male"}
                              onChange={() =>
                                setFormData({ ...formData, sex: "Male" })
                              }
                            />
                            Male
                          </label>
                          <label className="flex items-center gap-2 text-sm">
                            <input
                              type="radio"
                              name="sex"
                              checked={formData.sex === "Female"}
                              onChange={() =>
                                setFormData({ ...formData, sex: "Female" })
                              }
                            />
                            Female
                          </label>
                        </div>
                      </div>
                      <div>
                        <div className="text-sm font-semibold">
                          3. Date of Birth
                        </div>
                        <input
                          type="date"
                          className="w-full border border-gray-300 rounded px-2 py-1"
                          value={formData.dateOfBirth}
                          onChange={(e) =>
                            setFormData({
                              ...formData,
                              dateOfBirth: e.target.value,
                            })
                          }
                        />
                      </div>
                    </div>

                    <div>
                      <div className="text-sm font-semibold">
                        Place of Birth (Region/Zone/Woreda/Town/Kebele)
                      </div>
                      <input
                        className="w-full border-b border-gray-700 outline-none px-1 py-1"
                        value={formData.placeOfBirth}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            placeOfBirth: e.target.value,
                          })
                        }
                        placeholder="e.g. Amhara, ..."
                      />
                    </div>
                  </div>

                  <div className="border border-gray-700 p-3">
                    <div className="text-sm font-semibold mb-2">Photo</div>
                    <div className="w-full aspect-[3/4] border border-gray-400 bg-gray-50 flex items-center justify-center overflow-hidden">
                      {photoPreview ? (
                        // eslint-disable-next-line @next/next/no-img-element
                        <img
                          src={photoPreview}
                          alt="Student photo"
                          className="w-full h-full object-cover"
                        />
                      ) : (
                        <div className="text-xs text-gray-500 text-center px-2">
                          Upload a passport-size photo
                        </div>
                      )}
                    </div>
                    <input
                      type="file"
                      accept="image/*"
                      className="mt-3 w-full text-sm"
                      onChange={(e) =>
                        onPhotoFileChange(e.target.files?.[0] || null)
                      }
                    />
                    <div className="mt-2 text-xs text-gray-500">
                      Max 2MB. Stored with your form.
                    </div>
                  </div>
                </div>

                {/* Mother info */}
                <div className="border border-gray-800 p-4 space-y-3">
                  <div className="text-sm font-semibold">
                    4. Mother&apos;s/Adopter&apos;s full name
                  </div>
                  <input
                    className="w-full border-b border-gray-700 outline-none px-1 py-1"
                    value={formData.mothersFullName}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        mothersFullName: e.target.value,
                      })
                    }
                  />
                  <div className="text-sm font-semibold">Address</div>
                  <input
                    className="w-full border-b border-gray-700 outline-none px-1 py-1"
                    value={formData.mothersAddress}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        mothersAddress: e.target.value,
                      })
                    }
                    placeholder="Region/Zone/Woreda/Town/Kebele/Phone"
                  />
                </div>

                {/* School info */}
                <div className="border border-gray-800 p-4 space-y-3">
                  <div className="text-sm font-semibold">
                    5. School Name (Where you completed your preparatory
                    program)
                  </div>
                  <input
                    className="w-full border-b border-gray-700 outline-none px-1 py-1"
                    value={formData.schoolName}
                    onChange={(e) =>
                      setFormData({ ...formData, schoolName: e.target.value })
                    }
                  />
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                    <div>
                      <div className="text-sm font-semibold">
                        Date completed
                      </div>
                      <input
                        type="date"
                        className="w-full border border-gray-300 rounded px-2 py-1"
                        value={formData.dateCompleted}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            dateCompleted: e.target.value,
                          })
                        }
                      />
                    </div>
                    <div />
                  </div>
                </div>

                {/* University info */}
                <div className="border border-gray-800 p-4 space-y-3">
                  <div className="text-sm font-semibold">
                    6. University/College/Institute
                  </div>
                  <div className="text-sm">
                    Addis Ababa Science and Technology University
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                    <div>
                      <div className="text-sm font-semibold">
                        Faculty/College/Institute/School
                      </div>
                      <input
                        className="w-full border-b border-gray-700 outline-none px-1 py-1"
                        value={formData.facultyOrCollege}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            facultyOrCollege: e.target.value,
                          })
                        }
                        placeholder="e.g. Natural and Applied Sciences"
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">Department</div>
                      <input
                        className="w-full border-b border-gray-700 outline-none px-1 py-1"
                        value={
                          formData.department || selectedSlip.department || ""
                        }
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            department: e.target.value,
                          })
                        }
                      />
                    </div>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                    <div>
                      <div className="text-sm font-semibold">
                        Year of entrance (E.C.)
                      </div>
                      <input
                        className="w-full border-b border-gray-700 outline-none px-1 py-1"
                        value={formData.entranceYearEC}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            entranceYearEC: e.target.value,
                          })
                        }
                        placeholder="e.g. 2017 E.C."
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">
                        Year (I/II/III/IV/V)
                      </div>
                      <input
                        className="w-full border-b border-gray-700 outline-none px-1 py-1"
                        value={formData.academicYearText}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            academicYearText: e.target.value,
                          })
                        }
                        placeholder="e.g. IV"
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">Semester</div>
                      <input
                        className="w-full border-b border-gray-700 outline-none px-1 py-1"
                        value={formData.semesterText}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            semesterText: e.target.value,
                          })
                        }
                        placeholder="e.g. I"
                      />
                    </div>
                  </div>
                </div>

                {/* Page 2-ish: services + costs */}
                <div className="border-2 border-gray-800 p-4 space-y-4">
                  <div className="font-semibold">
                    9. What services would you demand? (mark “X”)
                  </div>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="border border-gray-400 p-3">
                      <div className="font-semibold text-sm mb-2">
                        A) In kind
                      </div>
                      <select
                        className="w-full border rounded px-2 py-1"
                        value={formData.serviceInKind}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            serviceInKind: e.target.value,
                          })
                        }
                      >
                        <option>Food only</option>
                        <option>Boarding only</option>
                        <option>Food and Boarding</option>
                      </select>
                    </div>
                    <div className="border border-gray-400 p-3">
                      <div className="font-semibold text-sm mb-2">
                        B) In cash
                      </div>
                      <select
                        className="w-full border rounded px-2 py-1"
                        value={formData.serviceInCash}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            serviceInCash: e.target.value,
                          })
                        }
                      >
                        <option>Food only</option>
                        <option>Boarding only</option>
                        <option>Food and Boarding</option>
                      </select>
                    </div>
                  </div>

                  <div className="font-semibold">10. Estimated cost (Birr)</div>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                    <div>
                      <div className="text-sm font-semibold">
                        15% tuition fee
                      </div>
                      <input
                        type="number"
                        step="0.01"
                        className="w-full border rounded px-2 py-1"
                        value={COSTS.tuitionFee15Percent}
                        readOnly
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">Food expense</div>
                      <input
                        type="number"
                        step="0.01"
                        className="w-full border rounded px-2 py-1"
                        value={COSTS.foodExpense}
                        readOnly
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">
                        Boarding expense
                      </div>
                      <input
                        type="number"
                        step="0.01"
                        className="w-full border rounded px-2 py-1"
                        value={COSTS.boardingExpense}
                        readOnly
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">Total</div>
                      <input
                        type="number"
                        step="0.01"
                        className="w-full border rounded px-2 py-1"
                        value={COSTS.totalCost}
                        readOnly
                      />
                    </div>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                    <div>
                      <div className="text-sm font-semibold">
                        11. Advance payment date
                      </div>
                      <input
                        type="date"
                        className="w-full border rounded px-2 py-1"
                        value={formData.advancePaymentDate}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            advancePaymentDate: e.target.value,
                          })
                        }
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">Discount</div>
                      <input
                        className="w-full border rounded px-2 py-1"
                        value={formData.discount}
                        onChange={(e) =>
                          setFormData({ ...formData, discount: e.target.value })
                        }
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">Receipt No.</div>
                      <input
                        className="w-full border rounded px-2 py-1"
                        value={formData.receiptNo}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            receiptNo: e.target.value,
                          })
                        }
                      />
                    </div>
                  </div>
                </div>

                {/* Sign + payment info */}
                <div className="border border-gray-800 p-4 space-y-3">
                  <div className="text-sm font-semibold">
                    Payment Information (details)
                  </div>
                  <textarea
                    value={formData.paymentInfo}
                    onChange={(e) =>
                      setFormData({ ...formData, paymentInfo: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                    rows={4}
                    placeholder="Receipt number, amount, date, etc."
                  />

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                    <div>
                      <div className="text-sm font-semibold">
                        Beneficiary name (for signature)
                      </div>
                      <input
                        className="w-full border-b border-gray-700 outline-none px-1 py-1"
                        value={formData.beneficiarySignatureName}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            beneficiarySignatureName: e.target.value,
                          })
                        }
                        placeholder="Type your full name"
                        required
                      />
                    </div>
                    <div>
                      <div className="text-sm font-semibold">
                        Signature date
                      </div>
                      <input
                        type="date"
                        className="w-full border rounded px-2 py-1"
                        value={formData.beneficiarySignedAt}
                        onChange={(e) =>
                          setFormData({
                            ...formData,
                            beneficiarySignedAt: e.target.value,
                          })
                        }
                        required
                      />
                    </div>
                  </div>
                </div>

                <button
                  type="submit"
                  className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                >
                  Submit Cost Sharing Form
                </button>
              </form>
            )}
          </div>
        )}

        <div className="mt-8">
          <h2 className="text-xl font-bold mb-4">My Cost Sharing Forms</h2>
          {loading ? (
            <div>Loading...</div>
          ) : forms.length === 0 ? (
            <div className="text-gray-600">No cost sharing forms found.</div>
          ) : (
            <table className="w-full border-collapse border">
              <thead>
                <tr className="bg-gray-100">
                  <th className="border px-4 py-2">Form ID</th>
                  <th className="border px-4 py-2">Status</th>
                  <th className="border px-4 py-2">Submitted At</th>
                  <th className="border px-4 py-2">Action</th>
                </tr>
              </thead>
              <tbody>
                {forms.map((form) => (
                  <tr key={form.id}>
                    <td className="border px-4 py-2">{form.id}</td>
                    <td className="border px-4 py-2">{form.status}</td>
                    <td className="border px-4 py-2">
                      {new Date(form.submittedAt).toLocaleString()}
                    </td>
                    <td className="border px-4 py-2">
                      <button
                        onClick={() => handleDownload(form.id)}
                        className="text-blue-600 hover:underline"
                      >
                        Download PDF
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </Layout>
  );
}
