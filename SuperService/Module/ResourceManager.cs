using BitMobile.ClientModel3;
using System.Collections.Generic;

namespace Test
{
    public static class ResourceManager
    {
        private const string ImageNotFound = @"Image\not_found.png";

        private static readonly Dictionary<string, object> Paths = new Dictionary<string, object>
        {
            {"tabbar_bag", @"Image\_Components\TabBar\Bag.png"},
            {"tabbar_bag_active", @"Image\_Components\TabBar\BagActive.png"},
            {"tabbar_clients", @"Image\_Components\TabBar\Clients.png"},
            {"tabbar_clients_active", @"Image\_Components\TabBar\ClientsActive.png"},
            {"tabbar_events", @"Image\_Components\TabBar\Events.png"},
            {"tabbar_events_active", @"Image\_Components\TabBar\EventsActive.png"},
            {"tabbar_settings", @"Image\_Components\TabBar\Settings.png"},
            {"tabbar_settings_active", @"Image\_Components\TabBar\SettingsActive.png"},
            //
            {"topinfo_downarrow", @"Image\_Components\TopInfoComponent\down_arrow_full_width.png"},
            {"topinfo_uparrow", @"Image\_Components\TopInfoComponent\up_arrow_full_width.png"},
            {"topinfo_downnoarrow", @"Image\_Components\TopInfoComponent\down_noarrow_full_width.png"},
            {"topinfo_upnoarrow", @"Image\_Components\TopInfoComponent\up_noarrow_full_width.png"},
            {"topinfo_extra_map", @"Image\top_map.png"},
            {"topinfo_extra_person", @"Image\top_person.png"},
            //
            {"topheading_back", @"Image\top_back.png"},
            {"topheading_filter", @"Image\top_eventlist_filtr_button.png"},
            {"topheading_map", @"Image\top_eventlist_map_button.png"},
            {"topheading_info", @"Image\top_info.png"},
            {"topheading_edit", @"Image\ClientScreen\top_ico_edit.png"},
            {"topheading_sync", @"Image\EventListScreen\Sinc.png" },
            //
            {"closeevent_wtb", @"Image\CloseEvent\Pokupka.png"},
            {"closeevent_wtb_selected", @"Image\CloseEvent\Pokupka_Selected.png"},
            {"closeevent_problem", @"Image\CloseEvent\Problema.png"},
            {"closeevent_problem_selected", @"Image\CloseEvent\Problema_Selected.png"},
            //
            {"tasklist_done", @"Image\TaskList\done.png"},
            {"tasklist_notdone", @"Image\TaskList\notDone.png"},
            {"tasklist_specdone", @"Image\TaskList\spec_done.png"},
            // EventListScreen
            {"eventlistscreen_blueborder", @"Image\EventListScreen\blue_border.png"},
            {"eventlistscreen_bluecircle", @"Image\EventListScreen\blue_circle.png"},
            {"eventlistscreen_bluedone", @"Image\EventListScreen\blue_done.png"},
            {"eventlistscreen_bluecancel", @"Image\EventListScreen\blue_cancel.png"},
            {"eventlistscreen_yellowborder", @"Image\EventListScreen\yellow_border.png"},
            {"eventlistscreen_yellowcircle", @"Image\EventListScreen\yellow_circle.png"},
            {"eventlistscreen_yellowdone", @"Image\EventListScreen\yellow_done.png"},
            {"eventlistscreen_yellowcancel", @"Image\EventListScreen\yellow_cancel.png"},
            {"eventlistscreen_redborder", @"Image\EventListScreen\red_border.png"},
            {"eventlistscreen_redcircle", @"Image\EventListScreen\red_circle.png"},
            {"eventlistscreen_reddone", @"Image\EventListScreen\red_done.png"},
            {"eventlistscreen_redcancel", @"Image\EventListScreen\red_cancel.png"},
            // EventScreen
            {"eventscreen_clist", @"Image\es_clist.png"},
            {"eventscreen_coc", @"Image\es_coc.png"},
            {"eventscreen_tasks", @"Image\es_tasks.png"},
            // ClientScreen
            {"clientscreen_phone", @"Image\ClientScreen\Phone.png"},
            {"clientscreen_plus", @"Image\ClientScreen\Plus.png"},
            {"clientscreen_gps", @"Image\ClientScreen\GPS.png"},
            //
            {"longtext_expand", @"Image\down_arrow_full_message.png"},
            {"longtext_close", @"Image\up_arrow_full_message.png"},
            //COC Screen
            {"cocscreen_delete", @"Image\COCScreen\delete_img.png"},
            {"cocscreen_plus", @"Image\COCScreen\plus_img.png"},
            //CheckListScreen
            {"checklistscreen_photo", @"Image\CheckListScreen\TakePhoto.png"},
            {"checklistscreen_nophoto", @"Image\CheckListScreen\NoPhoto.png"},
            //BagListScreen
            {"baglistscreen_busket", @"Image\BagListScreen\BagBusket.png"},
            {"baglistscreen_plus", @"Image\BagListScreen\BagPlus.png"},
            //CheckInfoScreen
            {"print_icon", @"Image\CheckInfoScreen\iconCheckPrint.png"},
            {"print_icon_disabel",@"Image\CheckInfoScreen\iconCheckPrint_dis.png" },
            // EditServicesOrMaterialsScreen
            {"editservicesormaterialsscreen_plus", @"Image\EditServicesOrMaterialsScreen\Plus.png"},
            {"editservicesormaterialsscreen_minus", @"Image\EditServicesOrMaterialsScreen\minus.png"},
            {"editservicesormaterialsscreen_minusdisabled", @"Image\EditServicesOrMaterialsScreen\minus_disable.png"},
            {"editservicesormaterialsscreen_close", @"Image\EditServicesOrMaterialsScreen\close.png"},
            // AuthScreen
            {"authscreen_logo", @"Image\AuthScreen\logo.png"},
            {"authscreen_password", @"Image\AuthScreen\password.png"},
            {"authscreen_username", @"Image\AuthScreen\username.png"},
            // Client Screen
            {"contactscreen_phone", @"Image\ContactScreen\Phone.png"},
            {"contactscreen_sms"  , @"Image\ContactScreen\sms.png"},
            {"contactscreen_email", @"Image\ContactScreen\email.png"},
            // EditContactScreen
            {"editcontactscreen_plus", @"Image\EditContactScreen\plus_img.png" },
            {"editcontactscreen_minus", @"Image\EditContactScreen\delete_img.png" },
            // PhotoScreen
            {"photoscreen_delete", @"Image\PhotoScreen\delete.png" },
            {"photoscreen_retake", @"Image\PhotoScreen\reload.png" },
            // Settings Screen
            {"settingsscreen_white_arrow",@"Image\SettingsScreen\bg_bottom.png" },
            {"settingsscreen_grey_arrow",@"Image\SettingsScreen\bg_top.png" },
            {"settingsscreen_company_logo",@"Image\SettingsScreen\Company.png" },
            {"settingsscreen_send_error",@"Image\SettingsScreen\Error.png" },
            {"settingsscreen_facebook",@"Image\SettingsScreen\FB.png" },
            {"settingsscreen_send_log",@"Image\SettingsScreen\log.png" },
            {"settingsscreen_send_logsend",@"Image\SettingsScreen\log_send.png" },
            {"settingsscreen_sservice_logo",@"Image\SettingsScreen\logo.png" },
            {"settingsscreen_logout",@"Image\SettingsScreen\logout.png" },
            {"settingsscreen_twitter",@"Image\SettingsScreen\Twitter.png" },
            {"settingsscreen_send_error_disable",@"Image\SettingsScreen\Error_disable.png" },
            {"settingsscreen_send_log_disable",@"Image\SettingsScreen\Log_Disable.png" },
            {"settingsscreen_userpic",@"Image\SettingsScreen\Userpic_blank.png" },
            {"settingsscreen_upload",@"Image\SettingsScreen\upload.png" },
            { "settingsscreen_printx", @"Image\SettingsScreen\X-check.png"},
            //TaskScreen
            {"task_target_done", @"Image\TaskScreen\done.png" },
            {"task_target_not_done", @"Image\TaskScreen\not_done.png" },
            //Print check screen
            {"printcheckscreen_white_printer", @"Image\PrintCheckScreen\IconPrintCheckWhite.png" },
            {"printcheckscreen_white_printer_diactivated", @"Image\PrintCheckScreen\IconPrintCheckGrey.png" },
        };

        public static string GetImage(string tag)
        {
            object res;
            if (!Paths.TryGetValue(tag, out res))
            {
                DConsole.WriteLine($"{tag} is not found in ResourceManager!");
                return ImageNotFound;
            }
            try
            {
                var stream = Application.GetResourceStream(res.ToString());
                stream.Close();
            }
            catch
            {
                DConsole.WriteLine($"{tag}:{res} does not exists!");
                return ImageNotFound;
            }
            return (string)res;
        }
    }
}