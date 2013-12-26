using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Graduation.Modules
{
    static class GetText
    {
        static public string StudentInfo()
        {
            List<string> fieldList = new List<string>();
            fieldList.Add("id as @ID");
            fieldList.Add("name as Name");
            fieldList.Add("english_name as EnglishName");
            fieldList.Add("birthdate as Birthdate");
            fieldList.Add("id_number as IDNumber");
            fieldList.Add("ref_class_id as RefClassID");
            fieldList.Add("birth_place as BirthPlace");
            fieldList.Add("mailing_address as MailingAddress");
            fieldList.Add("other_addresses as OtherAddress");
            fieldList.Add("permanent_address as PermanentAddress");
            fieldList.Add("nationality as Nationality");
            fieldList.Add("overseas_compatriot_from as OverseasCompatriotFrom");
            fieldList.Add("comment as Comment");
            fieldList.Add("ps_graduation_year as PS_GraduationYear");
            fieldList.Add("ethnic_group as EthnicGroup");
            fieldList.Add("graduate_year as GraduateYear");
            fieldList.Add("graduating_student_list_adn as GraduatingStudentListAdn");
            fieldList.Add("diploma_number as Diplomanumber");
            //fieldList.Add("certiflacte_type_obtained as CertiflacteTypeObtained"); //不存在
            fieldList.Add("email as Email");
            fieldList.Add("personal_website as PersonalWebSite");
            fieldList.Add("instant_message as InstantMessage");
            fieldList.Add("emergency_contact as EmergencyContact");
            fieldList.Add("grading_standard as GradingStandard");
            fieldList.Add("father_name as FatherName"); //父親
            fieldList.Add("father_living as FatherLiving");
            fieldList.Add("father_nationality as FatherNationality");
            fieldList.Add("father_other_info as FatherOtherInfo");
            fieldList.Add("mother_name as MotherName"); //母親
            fieldList.Add("mother_living as MotherLiving");
            fieldList.Add("mother_nationality as MotherNationality");
            fieldList.Add("mother_other_info as MotherOtherInfo");
            fieldList.Add("custodian_name as CustodianName"); //監護人
            fieldList.Add("custodian_nationality as CustodianNationality");
            //fieldList.Add("custodian_ralationship as CustodianRelationship"); //不存在
            fieldList.Add("custodian_other_info as CustodianOtherInfo");
            fieldList.Add("transcript as Transcript");
            fieldList.Add("student_number as StudentNumber");
            fieldList.Add("seat_no as SeatNo");
            fieldList.Add("status as Status");
            fieldList.Add("previous_school as PreviousSchool");
            fieldList.Add("permanent_phone as PermanentPhone");
            fieldList.Add("contact_phone as ContactPhone");
            fieldList.Add("other_phones as OtherPhones");
            fieldList.Add("enrollment_school_year as EnrollmentSchoolYear");
            fieldList.Add("enrollment_type as EnrollmentType");
            fieldList.Add("enrollment_category as EnrollmentCategory");
            fieldList.Add("enrollment_list_adn as EnrollmentListAdn");
            fieldList.Add("sibling_info as SiblingInfo");
            fieldList.Add("sms_phone as SmsPhone");
            fieldList.Add("grade_year as GradeYear");
            fieldList.Add("dept as Dept");
            fieldList.Add("ref_graduation_plan_id as RefGraduationPlanID");
            fieldList.Add("gender as Gender");
            fieldList.Add("grad_score as GradScore");
            fieldList.Add("previous_school_county as PreviousSchoolCounty");
            fieldList.Add("_enrollment_info as _EnrollmentInfo");
            fieldList.Add("_enrollment_synced as _EnrollmentSynced");
            fieldList.Add("sa_login_name as SaLoginName");
            fieldList.Add("sa_password as SaPassword");
            fieldList.Add("sems_history as SemsHistory");
            fieldList.Add("custodian_id_number as CustodianIDNumber");
            fieldList.Add("father_id_number as FatherIDNumber");
            fieldList.Add("mother_id_number as MotherIDNumber");
            fieldList.Add("_sa_password as _SaPassword");
            fieldList.Add("grad_rating as GradRating");
            fieldList.Add("ref_dept_id as RefDeptID");
            fieldList.Add("ref_score_calc_rule_id as RefScoreCalcRule");
            fieldList.Add("leave_info as LeaveInfo");
            fieldList.Add("account_type as AccountType");
            fieldList.Add("before_enrollment as BeforeEnrollment");
            return string.Join(",", fieldList.ToArray());
        }

        static public string  Attendance()
        {
            List<string> fieldList = new List<string>();
            fieldList.Add("id as @ID");
            fieldList.Add("ref_student_id as @RefStudentID");
            fieldList.Add("school_year as SchoolYear");
            fieldList.Add("semester as Semester");
            fieldList.Add("occur_date as OccurDate");
            fieldList.Add("detail as Detail");
            return string.Join(",", fieldList.ToArray());
        }

        static public string Discipline()
        {
            List<string> fieldList = new List<string>();
            fieldList.Add("id as @ID");
            fieldList.Add("ref_student_id as @RefStudentID");
            fieldList.Add("school_year as SchoolYear");
            fieldList.Add("semester as Semester");
            fieldList.Add("grade_year as GradeYear");
            fieldList.Add("occur_date as OccurDate");
            fieldList.Add("occur_place as OccurPlace");
            fieldList.Add("reason as Reason");
            fieldList.Add("merit_flag as MeritFlag");
            fieldList.Add("last_update_date as LastUpdateDate");
            fieldList.Add("register_date as RegisterDate");
            fieldList.Add("detail as Detail");
            fieldList.Add("type as type");
            return string.Join(",", fieldList.ToArray());
        }

        static public string UpdataRecord()
        {
            List<string> fieldList = new List<string>();
            fieldList.Add("id as @ID");
            fieldList.Add("ref_student_id as @RefStudentID");
            fieldList.Add("school_year as SchoolYear");
            fieldList.Add("semester as Semester");
            fieldList.Add("ss_name as SS_Name");
            fieldList.Add("ss_student_number as SS_StudentNumber");
            fieldList.Add("ss_gender as SS_Gender");
            fieldList.Add("ss_id_number as SS_ID_Number");
            fieldList.Add("ss_birthdate as SS_Birthdate");
            fieldList.Add("ss_grade_year as SS_GradeYear");
            fieldList.Add("ss_dept as Dept");
            fieldList.Add("update_date as UpdateDate");
            fieldList.Add("update_code as UpdateCode");
            fieldList.Add("update_type as UpdateType");
            fieldList.Add("update_reason as UpdateReason");
            fieldList.Add("update_desc as UpdateDesc");
            fieldList.Add("ad_date as ADDate");
            fieldList.Add("ad_number as ADNumber");
            fieldList.Add("last_ad_date as Last_ADDate");
            fieldList.Add("last_ad_number as Last_ADNumber");
            fieldList.Add("last_update_date as Last_UpdateDate");
            fieldList.Add("comment as Comment");
            fieldList.Add("context_info as ContextInfo");
            return string.Join(",", fieldList.ToArray());
        }

        static public string SemsMoralScore()
        {
            List<string> fieldList = new List<string>();
            fieldList.Add("id as @ID");
            fieldList.Add("ref_student_id as @RefStudentID");
            fieldList.Add("school_year as SchoolYear");
            fieldList.Add("semester as Semester");
            fieldList.Add("sb_diff as SB_Diff");
            fieldList.Add("sb_comment as SB_Comment");
            fieldList.Add("other_diff as OtherDiff");
            fieldList.Add("text_score as TextScore");
            fieldList.Add("initial_summary as InitialSummary");
            fieldList.Add("summary as Summary");
            return string.Join(",", fieldList.ToArray());
        }

        static public string SCAttend()
        {
            List<string> fieldList = new List<string>();
            fieldList.Add("id as @ID");
            fieldList.Add("ref_student_id as @RefStudentID");
            fieldList.Add("ref_course_id as @RefCourseID");
            fieldList.Add("is_required as IS_Required");
            fieldList.Add("score as Score");
            fieldList.Add("grade_year as GradeYear");
            fieldList.Add("required_by as Required_BY");
            fieldList.Add("extension as Extension");
            fieldList.Add("create_date as CreateDate");
            fieldList.Add("last_update as LastUpdate");
            return string.Join(",", fieldList.ToArray());
        }

        static public string SCETake()
        {
            List<string> fieldList = new List<string>();
            return string.Join(",", fieldList.ToArray());
        }
    }
}
