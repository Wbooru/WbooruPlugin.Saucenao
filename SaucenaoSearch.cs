using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Wbooru;
using Wbooru.Network;

namespace WbooruPlugin.Saucenao
{
    public static class SaucenaoSearch
    {
        #region Task Operations

        public static void BeginTask(SearchTask search_task)
        {
            Debug.Assert(SearchTask.CurrentSearchTask != search_task, "Not support multi tasks");
            if (search_task?.IsProcessing ?? true)
                return;

            search_task.IsProcessing = true;

            CancellationTokenSource source = new CancellationTokenSource();

            search_task.Report("等待任务执行...");

            var r = Task.Run(() => StartTask(search_task), source.Token);

            search_task.ProcessingTask = (r, source);
            SearchTask.CurrentSearchTask = search_task;
        }

        public static void StopTask(SearchTask search_task)
        {
            Debug.Assert(SearchTask.CurrentSearchTask != search_task, "Not support multi tasks");
            if (!(search_task?.IsProcessing ?? true))
                return;

            search_task.IsProcessing = true;

            if (!(search_task.ProcessingTask is (Task task, CancellationTokenSource source)))
                return;

            source.Cancel();
            SaveTask(search_task);

            SearchTask.CurrentSearchTask = null;
        }

        public static void SaveTask(SearchTask search_task)
        {
            if (string.IsNullOrWhiteSpace(search_task.FilePath))
                return;

            var content = JsonConvert.SerializeObject(search_task, Formatting.Indented);
            File.WriteAllText(search_task.FilePath, content);
        }

        private static void StartTask(SearchTask task)
        {
            task.Report("任务开始执行");
            task.Report("统计此任务完成程度...");

            var finished_instance = task.SearchImageInstances.Where(x => x.ResultInfos != null).ToArray();

            task.Report($"目前已完成 {finished_instance.Length} 个任务");
            task.Report(finished_instance);

            IEnumerable<SearchInstance> list;

            //RequestLimit limit = new RequestLimit();

            do
            {
                //limit.CheckLimit();

                list = task.SearchImageInstances.Where(x => x.ResultInfos == null);

                foreach (var instance in list)
                    if (TrySearch(task.Options, instance))
                        task.Report(instance);
                    else
                        task.Report($"Search image infomation ({instance.ImagePath}) failed,it will try again later...");
            } while (!list.Any());

            task.Report($"任务执行完成!");
        }

        #endregion

        public static bool TrySearch(SearchTaskOption option, SearchInstance instance)
        {
            const string base_url = "https://saucenao.com/search.php";
            var query_string = "?output_type=2";

            query_string += $"&api_key={option.APIOption.APIKey}";
            query_string += $"&db={option.APIOption.DB}";
            query_string += $"&testmode={(option.APIOption.TestMode ? 1 : 0)}";
            query_string += $"&numres={option.APIOption.RequestCount}";

            if (option.APIOption.DBMask != null)
                query_string += $"&dbmask={option.APIOption.DBMask}";

            if (option.APIOption.DBMaski != null)
                query_string += $"&dbmaski={option.APIOption.DBMaski}";

            var is_file_path = IsFilePath(instance.ImagePath);

            if (!is_file_path)
                query_string += $"&url={instance.ImagePath}";

            var url = base_url + query_string;

            try
            {
                var response = RequestHelper.CreateDeafult(url, req =>
                {
                    if (is_file_path)
                    {
                        req.Method = "POST";

                        using var content = new MultipartFormDataContent();

                        var file_content = new StreamContent(File.OpenRead(instance.ImagePath));
                        file_content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        content.Add(file_content, "file", $"{Path.GetFileName(instance.ImagePath)}");

                        var x = content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        
                        req.ContentType = content.Headers.ContentType.ToString();

                        x.CopyTo(req.GetRequestStream());   
                    }
                });

                var token = RequestHelper.GetJsonContainer<JObject>(response);

                var result = BuildSearchResults(token).OrderByDescending(x => x.Header.Similarity).ToArray();

                instance.ResultInfos = result;

                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Get image info {instance.ImagePath} failed: {e.Message}");
                instance.ResultInfos = null;
                return false;
            }
        }

        private static IEnumerable<ResultInfo> BuildSearchResults(JObject token)
        {
            if (!(token["results"] is JArray array))
                yield break;

            foreach (var r in array)
            {
                var result = r.ToObject<ResultInfo>();
                yield return result;
            }
        }

        private static bool IsFilePath(string imagePath)
        {
            if (imagePath.StartsWith("http:"))
                return false;

            return true;
        }
    }
}
