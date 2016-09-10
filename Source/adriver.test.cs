
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace NSCs_codegen {
    public partial class cs_codegenDriver {
#if OTHER_TEST
        static int invokeTestProc(SqlConnection conn) {
            int ret = -1;
            const string BADGE_NUM_SEB = "11111";
            const string BADGE_NUM_ERR = "00000";

            int rc;

            rc = callTestProc(BADGE_NUM_SEB, "OK", conn);
            rc = callTestProc(BADGE_NUM_ERR, "OK", conn);
            rc = callTestProc(null, "ERR1", conn);
            rc = callTestProc(null, "ERR2", conn);
            rc = callTestProc(null, "ERR3", conn);
            rc = callTestProc(null, "riktest", conn);
            return ret;
        }

        static int callTestProc(string badgeNo, string msg, SqlConnection conn) {
            const string PN_BADGE = "@badge_number";
            const string PN_MSG = "@message";
            int ret, rc;

            ret = -1;
            try {
                using (SqlCommand cmd = new SqlCommand("my_test_proc", conn)) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (string.IsNullOrEmpty(badgeNo))
                        cmd.Parameters.AddWithValue(PN_BADGE, DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue(PN_BADGE, badgeNo);
                    cmd.Parameters.AddWithValue(PN_MSG, msg);
                    Logger.logMethod(MethodBase.GetCurrentMethod());
                    if ((rc = cmd.ExecuteNonQuery()) > 0) {
                        ret = rc;
                        tryToReceiveData(cmd);
                    }
                }
            } catch (SqlException sex) {
                // seems like this SqlException has the print-messages in it.  I only wanna see 
                // the raise-error message.

                if (sex.Errors.Count > 0) {
                    SqlError e = sex.Errors[0];
                    Logger.logMethod(MethodBase.GetCurrentMethod(), Environment.NewLine +
                        "Server " + e.Server +
                        ", Error " + e.Number +
                        ", class " + e.Class +
                        " in procedure '" + e.Procedure + "', line " + e.LineNumber + Environment.NewLine +
                        "error-message: " + e.Message + Environment.NewLine);
                } else
                    Logger.logMethod(MethodBase.GetCurrentMethod(), sex);
            } catch (Exception ex) {
                Logger.logMethod(MethodBase.GetCurrentMethod(), ex);
            }
            return ret;
        }
#endif
    }
}