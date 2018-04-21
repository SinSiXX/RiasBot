# RiasBot
Rias is a general multi purpose bot with a lot of commands, anime, music, waifu, and administration. Written in C#, library Discord.Net.

Default prefix: `r!` (customizable) or `rias` (always)

# Commands

## 1. Administration
| Command | Description | Example |
| :---: | :---: | :---: |
| `ban` or `b` | Ban an user. If you provide a reson message the bot will DM the user with the reason. | `r!ban @User` or `r!b @User You've violated many rules!`
| `bye` | Togle announcements on the current channel when someone leaves the server. | `r!bye` |
| `byemsg` | Set the announcement message when someone leaves the server.<br>Placeholders:<br>`%user%` will mention the user who leaves<br>`%server%` or `%guild%` - will put the name of the server<br>Embeds are supported, json form: soon a site for it. | `r!byemsg %user% left...` |
| `greet` | Togle announcements on the current channel when someone joins the server. | `r!greet` |
| `greetmsg` | Set the announcement message when someone joins the server.<br>Placeholders:<br>`%user%` - will mention the user who joins<br>`%server%` or `%guild%` - will put the name of the server<br>Embeds are supported, json form: soon a site for it. | `r!greetmsg Welcome %user% to...` |
| `kick` or `k` | Kick an user. If you provide a reson message the bot will DM the user with the reason. | `r!k @User` or `r!k @User Your behavior is bad!` |
| `modlog` | Set the current channel as "mod-log" channel where the bot will post notifications about mute, unmute, kick, ban, softban. Provide no parameters to disable it from the current channel. | `r!modlog` |
| `mute` | Mute an user from text and voice channels. | `r!mute @User` |
| `prune` or `purge` | `r!prune x` removes last x number of messages from the channel (up to 100).<br>`r!prune @User` removes all User's messages in the last 100 messages.<br>`r!prune @Someone x` removes last x number of User's messages in the channel. | `r!prune x` or `r!prune @User` or `r!prune @User x` |
| `pruneban` or `pb` | Ban an user, remove his messages from last 7 days. Different from "softban" where the user is unbanned. | `r!pb` or `r!pb @User` or `r!pb @User DO NOT SPAM HERE!` |
| `setmute` | Set the mute role. If the role doesn't exists it will be created. | `r!setmute rias-mute` |
| `softban` or `sb` | Ban an user, remove his messages from last 7 days, and then unban it. | `r!sb @User` or `r!sb @User Don't spam here!` |
| `unmute` | Unmute an user from text and voice channels. | `r!unmute @User` |

### Channels
| Command | Description | Example |
| :---: | :---: | :---: |
| `channeltopic` or `ct` | Show the current channel's topic. | `r!ct` |
| `createcategory` or `ccat` | Create a category. | `r!ccat MEDIA` |
| `createchannel` or `cch` | Create a text or a voice channel. The type of the channel must be specified: `text` or `voice`. |
| `deletecategory` or `dcat` | Delete a category. | `r!dcat MEDIA` |
| `renamecategory` or `rncat` | Rename a category. You need to separate the old name by the new name with '->' | `r!rncat MEDIA -> MUSIC` |
| `renamechannel` or `rnch` | Rename a channel. You need to separate the old name by the new name with '->'. The name is case-sensitive. The type of the channel must be specified: `text` or `voice`. | `r!rnch text media -> music` or `r!rnch voice Media -> Music` |
| `setchanneltopic` or `sct` | Set this channel's topic | `r!sct topic` |

### Emotes
| Command | Description | Example |
| :---: | :---: | :---: |
| `addemote` | Add an emote on this server. PNG, JPG or GIF supported. The image must be under 256 KB. | `r!addemote https://i.imgur.com/fobETCs.png heart_diamond` |
| `renameemote` or `rnemote` | Rename an emote from this server. You need to separate the old name by the new name with '->' | `r!rnemote heart > heart_diamond` |
| `deleteemote` or `delemote` | Delete an emote from this server. | `r!delemote heart_diamond` |

### Roles
| Command | Description | Example |
| :---: | :---: | :---: |
| `autoassignablerole` or `aar` | Set a role to be auto-assigned for every user who joins the server. Provide no role to deactivate it. | `r!aar Newbie` |
| `createrole` or `cr` | Create a role. | `r!cr Rias` |
| `deleterole` or `dr` | Delete a role. | `r!dr Rias` |
| `hoistrole` or `hr` | Set whether or not a role should be displayed independently in the userlist. | `r!hr Admin` |
| `mentionrole` or `mr` | Set whether or not a role should be mentioned. | `r!mr Admin` |
| `removerole` or `rr` | Remove a role from an user. | `r!rr @User Admin` |
| `renamerole` or `rnr` | Rename a Role. You need to separate the old name by the new name with '->' | `r!rnr Rias -> Rias-waifu` |
| `rolecolor` or `rc` | Set a color for a role. | `r!rc color(hex format) roleName` or `r!rc ffffff Rias` |
| `roles` | Show the roles on this server. Pagination supported. | `r!roles` |
| `setrole` or `sr` | Set a role for an user. | `r!sr @User Admin` |

### SelfAssignableRoles
| Command | Description | Example |
| :---: | :---: | :---: |
| `asar` | Add a role in the server's self assginable roles list. | `r!asar kawaii` |
| `dsar` | Delete a role from the server's self assginable roles list. | `r!dsar kawaii` |
| `iam` | Assign to yourself a role from this server's self assignable roles list. | `r!iam kawaii` |
| `iamnot` or `iamn` | Remove from yourself a role from this server's self assignable roles list. | `r!iamn kawaii` |
| `lsar` | Show a list with the server's self assginable roles list. | `r!lsar` |

### Server
| Command | Description | Example |
| :---: | :---: | :---: |
| `nickname` or `nick` | Set an user's nickname in this server. Provide no parameters to remove the nickname. | `r!nickname @User nickname` |
| `setservericon` or `ssi` | Set this server's icon. | `r!ssr https://i.imgur.com/SrRA0Bp.png` |
| `setservername` or `ssn` | Set this server's name. | `r!ssr Rias Club` |

### Warning
| Command | Description | Example |
| :---: | :---: | :---: |
| `warnclear` or `clearwarn` | Clear an warning from an user. Type `all` to clear all warnings. | `r!warnclear @User 1` or `r!warnclear @User all` |
| `warning` or `warn` | Warn an user. Also, you can specify a reason. | `r!warn @User` or `r!warn @User Rule violated...` |
| `warninglist` or `warnlist` | Show a list with all warned users. | `r!warnlist` |
| `warninglog` or `warnlog` | Show all warnings for an user. | `r!warnlog @User` |
| `warnpunishment` or `warnpunish` | Set the number of warnings for a user to be punished and the punishment method (mute, kick, ban), or 0 to disable the punishment. Provide no parameters to show the current punishment method. | `r!warnpunish 0` or `r!warnpunish 3 kick` |

*writting...*