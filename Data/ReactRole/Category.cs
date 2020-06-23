using System;
using System.Collections.Generic;
using System.Text;

namespace siscode_bot.Data.ReactRole {
    public class Category {
        public int Id { get; set; }
        public ulong MessageId { get; set; }
        public ulong ChannelId { get; set; }
        public string Name { get; set; }
        public Dictionary<ulong, ulong> Data { get; set; } = new Dictionary<ulong, ulong>();
    }
}
