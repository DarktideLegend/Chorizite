local rx = require('rx')
local request = require('request')
local Net = require(typeof(CS.Chorizite.Core.Net.NetworkParser))
local ac = require('Plugins.Core.AC').Game
local utils = require('utils')

local state = rx:CreateState({
  collectingInboxItems = false,
  items = nil,
  HandleGetInboxItemsResponse = function(self, response)
    self.items = nil;
    if response.Success then
      self.items = response.Data
    else
      print(response.ErrorMessage)
    end
  end
})

local onInboxNotification = utils.debounce(function()
  print("[InboxItems] -> InboxNotificationResponse Event Handler")
  request.fetchInboxItems();
end, 3000)

local OpCodeHandlers = {
  [0x10006] = function(evt)
    print("[InboxItems] -> GetInboxItemsResponse Event Handler")
    local getInboxItemsResponse = request.read(evt.RawData)
    state.HandleGetInboxItemsResponse(getInboxItemsResponse)
  end,
  [0x10007] = onInboxNotification,
  [0x10008] = function(evt)
    print("[InboxItems] -> CollectInboxItemsResponse Event Handler")
    request.fetchInboxItems();
  end
}

local onMount = function()
  print("MOUNTED INBOX")
  Net.Messages:OnUnknownMessage('+', function(sender, evt)
    if OpCodeHandlers[evt.OpCode] then
      OpCodeHandlers[evt.OpCode](evt)
    end
  end)
  request.fetchInboxItems()
end

local InboxListItemInfo = function(item)
  return rx:Div({ class = "inbox-list-item-info-container" }, {
    rx:Div({ class = "inbox-list-item-info-from" }, item.From),
    rx:Div({ class = "inbox-list-item-info-subject" }, item.Subject),
  })
end

local InboxListItem = function(item)
  local itemIcon = string.format(
    "dat://0x%08X?underlay=0x%08X&overlay=0x%08X&uieffect=%s",
    tonumber(item.IconId),
    0,
    0,
    "")

  return rx:Div({ class = "inbox-list-item" }, {
    rx:Div({
      class = "icon-stack",
    }, {
      rx:Div({
        class = "inbox-list-item-icon icon-item",
        style = string.format("decorator: image( %s )", itemIcon),
      })
    }),
    InboxListItemInfo(item),
    rx:Div({ class = "inbox-list-item-age" }),
  })
end

local InboxItemsContent = function(state)
  if state.items == nil then
    return rx:Div("... loading")
  end

  local ret = {}
  for i, item in ipairs(state.items) do
    table.insert(ret, InboxListItem(item))
  end
  return table.unpack(ret)
end

local InboxItems = function(state)
  return rx:Div({ class = "inbox-items" }, {
    InboxItemsContent(state)
  })
end

local InboxItemsPagination = function(state)
  return rx:Div({ class = "inbox-items-pagination" }, {
    rx:Button({
      class = "primary inbox-items-collect-all",
      onClick = function()
        if not state.collectingInboxItems then
          request.collectInboxItems();
        end
      end
    }, "Collect All")
  })
end

local InboxView = function(state)
  print("RENDERED INBOX")
  return rx:Div({
    class = "inbox-container",
    onMount = onMount
  }, {
    InboxItems(state),
    InboxItemsPagination(state)
  })
end

document:Mount(function() return InboxView(state) end, "#inbox")
