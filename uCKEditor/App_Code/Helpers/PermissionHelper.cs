using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;

namespace uCKEditor.Helpers
{

    public class PermissionHelper
    {

        public static bool CheckContentPermissions(int userId, int nodeId, char[] permissionsToCheck = null)
        {
            if (nodeId == Constants.System.Root || nodeId == Constants.System.RecycleBinContent)
            {
                return false;
            }

            var contentItem = ApplicationContext.Current.Services.ContentService.GetById(nodeId);
            if (contentItem == null)
            {
                return false;
            }

            var user = ApplicationContext.Current.Services.UserService.GetUserById(userId);
            var hasPathAccess = (nodeId == Constants.System.Root)
                                    ? UserExtensions.HasPathAccess(
                                        Constants.System.Root.ToInvariantString(),
                                        user.StartContentId,
                                        Constants.System.RecycleBinContent)
                                    : (nodeId == Constants.System.RecycleBinContent)
                                          ? UserExtensions.HasPathAccess(
                                              Constants.System.RecycleBinContent.ToInvariantString(),
                                              user.StartContentId,
                                              Constants.System.RecycleBinContent)
                                          : user.HasPathAccess(contentItem);
            if (!hasPathAccess)
            {
                return false;
            }

            if (permissionsToCheck == null || permissionsToCheck.Any() == false)
            {
                return true;
            }

            var permission = ApplicationContext.Current.Services.UserService.GetPermissions(user, nodeId).FirstOrDefault();
            var allowed = true;
            foreach (var p in permissionsToCheck)
            {
                if (permission == null || permission.AssignedPermissions.Contains(p.ToString(CultureInfo.InvariantCulture)) == false)
                {
                    allowed = false;
                    break;
                }
            }

            return allowed;
        }

        public static bool CheckMediaPermissions(int userId, int nodeId, char[] permissionsToCheck = null)
        {
            if (nodeId == Constants.System.Root || nodeId == Constants.System.RecycleBinMedia)
            {
                return false;
            }

            var mediaItem = ApplicationContext.Current.Services.MediaService.GetById(nodeId);
            if (mediaItem == null)
            {
                return false;
            }

            var user = ApplicationContext.Current.Services.UserService.GetUserById(userId);
            var hasPathAccess = (nodeId == Constants.System.Root)
                                    ? UserExtensions.HasPathAccess(
                                        Constants.System.Root.ToInvariantString(),
                                        user.StartMediaId,
                                        Constants.System.RecycleBinMedia)
                                    : (nodeId == Constants.System.RecycleBinMedia)
                                          ? UserExtensions.HasPathAccess(
                                              Constants.System.RecycleBinMedia.ToInvariantString(),
                                              user.StartMediaId,
                                              Constants.System.RecycleBinMedia)
                                          : user.HasPathAccess(mediaItem);

            return hasPathAccess;
        }

    }
}